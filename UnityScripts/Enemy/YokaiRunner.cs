using UnityEngine;
using ShogunsLegacy.Data;
using ShogunsLegacy.Interfaces;

namespace ShogunsLegacy.Enemy
{
    /// <summary>
    /// Fast, aggressive enemy that dashes at the player
    /// Low HP but difficult to hit due to speed
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(HealthComponent))]
    public class YokaiRunner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameStats stats;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trail;

        [Header("Detection")]
        [SerializeField] private float detectionRange = 12f;
        [SerializeField] private float loseTargetRange = 18f;

        [Header("Movement")]
        [SerializeField] private float normalSpeed = 4f;
        [SerializeField] private float dashSpeed = 12f;
        [SerializeField] private float acceleration = 30f;

        [Header("Dash Attack")]
        [SerializeField] private float dashWindupTime = 0.5f; // Time before dashing
        [SerializeField] private float dashDuration = 0.4f; // How long dash lasts
        [SerializeField] private float dashCooldown = 2f;
        [SerializeField] private float dashRange = 8f; // Only dash if player is within range
        [SerializeField] private float dashDamage = 12f;
        [SerializeField] private bool dashThroughTarget = true; // Passes through player

        [Header("Circle Strategy")]
        [SerializeField] private bool circleBeforeDash = true;
        [SerializeField] private float circleDistance = 5f; // Distance to circle around player
        [SerializeField] private float circleSpeed = 3f;
        [SerializeField] private float circleDuration = 1.5f; // How long to circle before dashing

        [Header("Evasion")]
        [SerializeField] private bool canEvade = true;
        [SerializeField] private float evadeDistance = 2f;
        [SerializeField] private float evadeSpeed = 8f;
        [SerializeField] private float evadeCooldown = 3f;

        [Header("VFX")]
        [SerializeField] private GameObject dashVFXPrefab;
        [SerializeField] private GameObject afterImagePrefab; // Trail effect during dash

        [Header("Loot")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private int coinsDropped = 4;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool showGizmos = true;

        // Components
        private Rigidbody2D rb;
        private HealthComponent healthComponent;
        private Animator animator;
        private Collider2D enemyCollider;

        // State
        private RunnerState currentState = RunnerState.Idle;
        private Transform target;
        private Vector2 currentVelocity;
        private Vector2 dashDirection;
        private float dashCooldownTimer = 0f;
        private float evadeCooldownTimer = 0f;
        private float stateTimer = 0f;
        private float circleAngle = 0f;

        // Animation hashes
        private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
        private static readonly int HashDash = Animator.StringToHash("Dash");
        private static readonly int HashHurt = Animator.StringToHash("Hurt");
        private static readonly int HashDeath = Animator.StringToHash("Death");

        #region Unity Lifecycle

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            healthComponent = GetComponent<HealthComponent>();
            animator = GetComponent<Animator>();
            enemyCollider = GetComponent<Collider2D>();

            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.freezeRotation = true;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        private void Start()
        {
            if (stats != null)
            {
                healthComponent.SetMaxHealth(stats.maxHealth, true);
                normalSpeed = stats.moveSpeed;
                dashDamage = stats.baseDamage;
            }

            healthComponent.OnDamageTaken.AddListener(OnDamageTaken);
            healthComponent.OnDeath.AddListener(OnDeath);

            // Enable trail
            if (trail != null)
            {
                trail.emitting = false;
            }
        }

        private void Update()
        {
            if (!healthComponent.IsAlive || target == null) return;

            UpdateTimers();
            UpdateState();
            ExecuteState();
        }

        private void FixedUpdate()
        {
            if (!healthComponent.IsAlive) return;
            rb.velocity = currentVelocity;
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
            {
                healthComponent.OnDamageTaken.RemoveListener(OnDamageTaken);
                healthComponent.OnDeath.RemoveListener(OnDeath);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (currentState == RunnerState.Dashing)
            {
                CheckDashHit(collision.gameObject);
            }
        }

        #endregion

        #region State Machine

        private void UpdateState()
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            switch (currentState)
            {
                case RunnerState.Idle:
                    if (distanceToTarget <= detectionRange)
                    {
                        ChangeState(circleBeforeDash ? RunnerState.Circling : RunnerState.Chasing);
                    }
                    break;

                case RunnerState.Chasing:
                    if (distanceToTarget > loseTargetRange)
                    {
                        ChangeState(RunnerState.Idle);
                    }
                    else if (distanceToTarget <= dashRange && dashCooldownTimer <= 0)
                    {
                        ChangeState(RunnerState.Winding);
                    }
                    break;

                case RunnerState.Circling:
                    if (distanceToTarget > loseTargetRange)
                    {
                        ChangeState(RunnerState.Idle);
                    }
                    else if (stateTimer <= 0 && dashCooldownTimer <= 0)
                    {
                        ChangeState(RunnerState.Winding);
                    }
                    break;

                case RunnerState.Winding:
                    if (stateTimer <= 0)
                    {
                        ChangeState(RunnerState.Dashing);
                    }
                    break;

                case RunnerState.Dashing:
                    if (stateTimer <= 0)
                    {
                        ChangeState(RunnerState.Chasing);
                    }
                    break;

                case RunnerState.Evading:
                    if (stateTimer <= 0)
                    {
                        ChangeState(RunnerState.Chasing);
                    }
                    break;
            }
        }

        private void ExecuteState()
        {
            switch (currentState)
            {
                case RunnerState.Idle:
                    StateIdle();
                    break;

                case RunnerState.Chasing:
                    StateChasing();
                    break;

                case RunnerState.Circling:
                    StateCircling();
                    break;

                case RunnerState.Winding:
                    StateWindingUp();
                    break;

                case RunnerState.Dashing:
                    StateDashing();
                    break;

                case RunnerState.Evading:
                    StateEvading();
                    break;
            }
        }

        private void ChangeState(RunnerState newState)
        {
            if (currentState == newState) return;

            if (debugMode)
            {
                Debug.Log($"[YokaiRunner] {name}: {currentState} -> {newState}");
            }

            currentState = newState;

            // State enter logic
            switch (newState)
            {
                case RunnerState.Circling:
                    stateTimer = circleDuration;
                    circleAngle = Random.Range(0f, 360f);
                    break;

                case RunnerState.Winding:
                    stateTimer = dashWindupTime;
                    currentVelocity = Vector2.zero;
                    dashDirection = (target.position - transform.position).normalized;
                    break;

                case RunnerState.Dashing:
                    stateTimer = dashDuration;
                    dashCooldownTimer = dashCooldown;
                    StartDash();
                    break;

                case RunnerState.Evading:
                    stateTimer = 0.3f; // Quick evade
                    evadeCooldownTimer = evadeCooldown;
                    break;
            }
        }

        #endregion

        #region States

        private void StateIdle()
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, acceleration * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool(HashIsMoving, false);
            }
        }

        private void StateChasing()
        {
            Vector2 direction = (target.position - transform.position).normalized;
            Vector2 targetVelocity = direction * normalSpeed;

            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool(HashIsMoving, true);
            }

            UpdateSpriteDirection(direction);
        }

        private void StateCircling()
        {
            // Circle around player at set distance
            Vector2 toTarget = (target.position - transform.position);
            float currentDistance = toTarget.magnitude;

            // Move to circle distance if not there yet
            if (Mathf.Abs(currentDistance - circleDistance) > 0.5f)
            {
                Vector2 adjustDirection = currentDistance > circleDistance ? -toTarget.normalized : toTarget.normalized;
                currentVelocity = adjustDirection * normalSpeed;
            }
            else
            {
                // Circle around
                circleAngle += circleSpeed * Time.deltaTime * 50f; // Degrees per second
                Vector2 circleOffset = new Vector2(
                    Mathf.Cos(circleAngle * Mathf.Deg2Rad),
                    Mathf.Sin(circleAngle * Mathf.Deg2Rad)
                ) * circleDistance;

                Vector2 targetPos = (Vector2)target.position + circleOffset;
                Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

                currentVelocity = direction * circleSpeed;
            }

            if (animator != null)
            {
                animator.SetBool(HashIsMoving, true);
            }

            UpdateSpriteDirection(currentVelocity);

            stateTimer -= Time.deltaTime;
        }

        private void StateWindingUp()
        {
            // Stand still and prepare to dash
            currentVelocity = Vector2.zero;

            // Visual telegraph - could flash sprite or play animation
            if (spriteRenderer != null && stateTimer < 0.2f)
            {
                // Flash effect
                float flashSpeed = 20f;
                spriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * flashSpeed, 1f));
            }

            stateTimer -= Time.deltaTime;
        }

        private void StateDashing()
        {
            // Move at high speed in dash direction
            currentVelocity = dashDirection * dashSpeed;

            // Enable trail
            if (trail != null)
            {
                trail.emitting = true;
            }

            stateTimer -= Time.deltaTime;
        }

        private void StateEvading()
        {
            // Quick dash away from player
            Vector2 evadeDirection = (transform.position - target.position).normalized;
            currentVelocity = evadeDirection * evadeSpeed;

            if (trail != null)
            {
                trail.emitting = true;
            }

            stateTimer -= Time.deltaTime;
        }

        #endregion

        #region Dash Attack

        private void StartDash()
        {
            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger(HashDash);
            }

            // Spawn dash VFX
            if (dashVFXPrefab != null)
            {
                GameObject vfx = Instantiate(dashVFXPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            // Make invulnerable during dash (optional)
            if (healthComponent != null)
            {
                healthComponent.SetInvulnerable(dashDuration);
            }

            if (debugMode)
            {
                Debug.Log($"[YokaiRunner] {name} dashing!");
            }
        }

        private void CheckDashHit(GameObject target)
        {
            if (((1 << target.layer) & playerLayer) == 0)
                return;

            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(dashDamage, gameObject);

                if (debugMode)
                {
                    Debug.Log($"[YokaiRunner] Dash hit {target.name}!");
                }

                if (!dashThroughTarget)
                {
                    // Stop dash
                    ChangeState(RunnerState.Chasing);
                }
            }
        }

        #endregion

        #region Health Events

        private void OnDamageTaken(float damage, GameObject source)
        {
            if (animator != null)
            {
                animator.SetTrigger(HashHurt);
            }

            // Restore sprite color (in case it was flashing)
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }

            // Maybe evade after being hit
            if (canEvade && evadeCooldownTimer <= 0 && Random.value < 0.5f)
            {
                ChangeState(RunnerState.Evading);
            }
            else if (currentState == RunnerState.Idle)
            {
                ChangeState(RunnerState.Chasing);
            }
        }

        private void OnDeath()
        {
            currentState = RunnerState.Dead;
            currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;

            if (animator != null)
            {
                animator.SetTrigger(HashDeath);
            }

            if (trail != null)
            {
                trail.emitting = false;
            }

            rb.simulated = false;
            enabled = false;

            DropLoot();
            Destroy(gameObject, 2f);

            if (debugMode)
            {
                Debug.Log($"[YokaiRunner] {name} died!");
            }
        }

        #endregion

        #region Utility

        private void UpdateSpriteDirection(Vector2 direction)
        {
            if (spriteRenderer != null && direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        private void UpdateTimers()
        {
            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= Time.deltaTime;
            }

            if (evadeCooldownTimer > 0)
            {
                evadeCooldownTimer -= Time.deltaTime;
            }

            // Disable trail when not dashing/evading
            if (trail != null && currentState != RunnerState.Dashing && currentState != RunnerState.Evading)
            {
                trail.emitting = false;
            }

            // Restore sprite color
            if (spriteRenderer != null && currentState != RunnerState.Winding)
            {
                spriteRenderer.color = Color.white;
            }
        }

        private void DropLoot()
        {
            if (coinPrefab == null) return;

            for (int i = 0; i < coinsDropped; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
                Vector3 spawnPos = transform.position + (Vector3)randomOffset;

                GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

                Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
                if (coinRb != null)
                {
                    Vector2 randomForce = Random.insideUnitCircle * 2f;
                    coinRb.AddForce(randomForce, ForceMode2D.Impulse);
                }
            }
        }

        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Dash range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, dashRange);

            // Circle distance
            if (circleBeforeDash)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, circleDistance);
            }

            // Dash direction
            if (currentState == RunnerState.Winding || currentState == RunnerState.Dashing)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)dashDirection * 5f);
            }

            // Line to target
            if (target != null && currentState != RunnerState.Idle)
            {
                Gizmos.color = currentState == RunnerState.Dashing ? Color.red : Color.white;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }

        #endregion
    }

    public enum RunnerState
    {
        Idle,
        Chasing,
        Circling,
        Winding,  // Wind-up before dash
        Dashing,
        Evading,
        Dead
    }
}
