using UnityEngine;
using ShogunsLegacy.Data;
using ShogunsLegacy.Combat;

namespace ShogunsLegacy.Enemy
{
    /// <summary>
    /// Ranged enemy that maintains distance and shoots projectiles
    /// More tactical AI than basic melee enemy
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(HealthComponent))]
    public class OniArcher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameStats stats;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Detection")]
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float loseTargetRange = 15f;

        [Header("Range Management")]
        [SerializeField] private float preferredMinRange = 5f; // Stay at least this far
        [SerializeField] private float preferredMaxRange = 8f; // But not further than this
        [SerializeField] private float tooCloseRange = 3f; // Flee if player gets this close

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float fleeSpeed = 4f; // Faster when fleeing
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float strafeChance = 0.3f; // Chance to strafe instead of backing up

        [Header("Combat")]
        [SerializeField] private float shootCooldown = 2f;
        [SerializeField] private float projectileSpeed = 8f;
        [SerializeField] private float projectileDamage = 8f;
        [SerializeField] private int burstCount = 1; // Number of shots per attack
        [SerializeField] private float burstDelay = 0.3f; // Delay between burst shots
        [SerializeField] private float aimTime = 0.5f; // Time to aim before shooting

        [Header("Behavior")]
        [SerializeField] private bool canReposition = true;
        [SerializeField] private float repositionInterval = 3f; // Reposition every X seconds
        [SerializeField] private float repositionDistance = 3f;

        [Header("Loot")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private int coinsDropped = 5; // More coins than melee enemy

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool showGizmos = true;

        // Components
        private Rigidbody2D rb;
        private HealthComponent healthComponent;
        private Animator animator;

        // State
        private ArcherState currentState = ArcherState.Idle;
        private Transform target;
        private Vector2 currentVelocity;
        private float shootCooldownTimer = 0f;
        private float repositionTimer = 0f;
        private int currentBurstShot = 0;
        private bool isAiming = false;
        private float aimTimer = 0f;
        private Vector3 repositionTarget;

        // Animation hashes
        private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
        private static readonly int HashAttack = Animator.StringToHash("Attack");
        private static readonly int HashHurt = Animator.StringToHash("Hurt");
        private static readonly int HashDeath = Animator.StringToHash("Death");

        #region Unity Lifecycle

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            healthComponent = GetComponent<HealthComponent>();
            animator = GetComponent<Animator>();

            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.freezeRotation = true;

            // Find player
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        private void Start()
        {
            // Apply stats
            if (stats != null)
            {
                healthComponent.SetMaxHealth(stats.maxHealth, true);
                moveSpeed = stats.moveSpeed;
                projectileDamage = stats.baseDamage;
            }

            healthComponent.OnDamageTaken.AddListener(OnDamageTaken);
            healthComponent.OnDeath.AddListener(OnDeath);

            repositionTimer = repositionInterval;
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

        #endregion

        #region State Machine

        private void UpdateState()
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            switch (currentState)
            {
                case ArcherState.Idle:
                    if (distanceToTarget <= detectionRange)
                    {
                        ChangeState(ArcherState.Positioning);
                    }
                    break;

                case ArcherState.Positioning:
                    if (distanceToTarget > loseTargetRange)
                    {
                        ChangeState(ArcherState.Idle);
                    }
                    else if (distanceToTarget < tooCloseRange)
                    {
                        ChangeState(ArcherState.Fleeing);
                    }
                    else if (IsInPreferredRange(distanceToTarget) && shootCooldownTimer <= 0)
                    {
                        ChangeState(ArcherState.Aiming);
                    }
                    break;

                case ArcherState.Fleeing:
                    if (distanceToTarget >= preferredMinRange)
                    {
                        ChangeState(ArcherState.Positioning);
                    }
                    break;

                case ArcherState.Aiming:
                    // Stay in aiming until aim time completes
                    break;

                case ArcherState.Shooting:
                    // Shooting handled by coroutine
                    break;
            }
        }

        private void ExecuteState()
        {
            switch (currentState)
            {
                case ArcherState.Idle:
                    StateIdle();
                    break;

                case ArcherState.Positioning:
                    StatePositioning();
                    break;

                case ArcherState.Fleeing:
                    StateFleeing();
                    break;

                case ArcherState.Aiming:
                    StateAiming();
                    break;

                case ArcherState.Shooting:
                    // Handled by coroutine
                    break;
            }
        }

        private void ChangeState(ArcherState newState)
        {
            if (currentState == newState) return;

            if (debugMode)
            {
                Debug.Log($"[OniArcher] {name}: {currentState} -> {newState}");
            }

            currentState = newState;

            // State enter logic
            if (newState == ArcherState.Aiming)
            {
                isAiming = true;
                aimTimer = aimTime;
                currentVelocity = Vector2.zero;
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

        private void StatePositioning()
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            Vector2 moveDirection = Vector2.zero;

            // Too far - move closer
            if (distanceToTarget > preferredMaxRange)
            {
                moveDirection = directionToTarget;
            }
            // Too close - back up
            else if (distanceToTarget < preferredMinRange)
            {
                moveDirection = -directionToTarget;

                // Chance to strafe instead
                if (Random.value < strafeChance)
                {
                    moveDirection = Vector2.Perpendicular(directionToTarget) * (Random.value > 0.5f ? 1f : -1f);
                }
            }
            // In range - occasional repositioning
            else if (canReposition && repositionTimer <= 0)
            {
                Vector2 perpendicular = Vector2.Perpendicular(directionToTarget);
                moveDirection = perpendicular * (Random.value > 0.5f ? 1f : -1f);
                repositionTimer = repositionInterval;
            }

            // Apply movement
            if (moveDirection.magnitude > 0.1f)
            {
                Vector2 targetVelocity = moveDirection.normalized * moveSpeed;
                currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

                if (animator != null)
                {
                    animator.SetBool(HashIsMoving, true);
                }
            }
            else
            {
                currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, acceleration * Time.deltaTime);

                if (animator != null)
                {
                    animator.SetBool(HashIsMoving, false);
                }
            }

            // Face target
            UpdateSpriteDirection(directionToTarget);
        }

        private void StateFleeing()
        {
            Vector2 fleeDirection = (transform.position - target.position).normalized;

            // Add some randomness to flee direction
            fleeDirection += Random.insideUnitCircle * 0.3f;
            fleeDirection.Normalize();

            Vector2 targetVelocity = fleeDirection * fleeSpeed;
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool(HashIsMoving, true);
            }

            UpdateSpriteDirection(fleeDirection);
        }

        private void StateAiming()
        {
            currentVelocity = Vector2.zero;

            aimTimer -= Time.deltaTime;
            if (aimTimer <= 0)
            {
                StartShooting();
            }

            // Face target while aiming
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            UpdateSpriteDirection(directionToTarget);
        }

        #endregion

        #region Combat

        private void StartShooting()
        {
            ChangeState(ArcherState.Shooting);
            StartCoroutine(ShootBurst());
        }

        private System.Collections.IEnumerator ShootBurst()
        {
            for (int i = 0; i < burstCount; i++)
            {
                ShootProjectile();

                if (animator != null)
                {
                    animator.SetTrigger(HashAttack);
                }

                if (i < burstCount - 1)
                {
                    yield return new WaitForSeconds(burstDelay);
                }
            }

            shootCooldownTimer = shootCooldown;
            isAiming = false;
            ChangeState(ArcherState.Positioning);
        }

        private void ShootProjectile()
        {
            if (projectilePrefab == null || target == null) return;

            Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
            Vector2 direction = (target.position - spawnPos).normalized;

            // Instantiate projectile
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            // Initialize projectile
            ProjectileController projectile = projectileObj.GetComponent<ProjectileController>();
            if (projectile != null)
            {
                projectile.Initialize(direction, gameObject, projectileDamage);
                projectile.SetSpeed(projectileSpeed);
                projectile.SetTargetLayers(playerLayer);
            }

            if (debugMode)
            {
                Debug.Log($"[OniArcher] {name} shot projectile!");
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

            // React to damage - might want to flee
            if (currentState == ArcherState.Idle)
            {
                ChangeState(ArcherState.Positioning);
            }
        }

        private void OnDeath()
        {
            currentState = ArcherState.Dead;
            currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;

            if (animator != null)
            {
                animator.SetTrigger(HashDeath);
            }

            rb.simulated = false;
            enabled = false;

            DropLoot();
            Destroy(gameObject, 2f);

            if (debugMode)
            {
                Debug.Log($"[OniArcher] {name} died!");
            }
        }

        #endregion

        #region Utility

        private bool IsInPreferredRange(float distance)
        {
            return distance >= preferredMinRange && distance <= preferredMaxRange;
        }

        private void UpdateSpriteDirection(Vector2 direction)
        {
            if (spriteRenderer != null && direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        private void UpdateTimers()
        {
            if (shootCooldownTimer > 0)
            {
                shootCooldownTimer -= Time.deltaTime;
            }

            if (repositionTimer > 0)
            {
                repositionTimer -= Time.deltaTime;
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

            // Preferred min range
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, preferredMinRange);

            // Preferred max range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, preferredMaxRange);

            // Too close range (flee)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, tooCloseRange);

            // Line to target
            if (target != null && currentState != ArcherState.Idle)
            {
                Gizmos.color = currentState == ArcherState.Shooting || currentState == ArcherState.Aiming ? Color.red : Color.white;
                Gizmos.DrawLine(transform.position, target.position);
            }

            // Shoot point
            if (shootPoint != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(shootPoint.position, 0.2f);
            }
        }

        #endregion
    }

    public enum ArcherState
    {
        Idle,
        Positioning,
        Fleeing,
        Aiming,
        Shooting,
        Dead
    }
}
