using UnityEngine;
using ShogunsLegacy.Data;
using ShogunsLegacy.Interfaces;

namespace ShogunsLegacy.Enemy
{
    /// <summary>
    /// Basic enemy AI with state machine
    /// States: Idle, Chase, Attack, Death
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameStats stats;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Detection")]
        [SerializeField] private float detectionRange = 8f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float loseTargetRange = 12f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float acceleration = 20f;

        [Header("Combat")]
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackWindup = 0.3f; // Time before hit
        [SerializeField] private float attackDuration = 0.5f; // Total attack animation time

        [Header("Loot")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private int coinsDropped = 3;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool showGizmos = true;

        // Components
        private Rigidbody2D rb;
        private HealthComponent healthComponent;
        private Animator animator;

        // State
        private EnemyState currentState = EnemyState.Idle;
        private Transform target; // Player
        private Vector2 currentVelocity;
        private float attackCooldownTimer = 0f;
        private bool isAttacking = false;

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

            // Configure Rigidbody2D
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
                attackDamage = stats.baseDamage;
            }

            // Subscribe to health events
            healthComponent.OnDamageTaken.AddListener(OnDamageTaken);
            healthComponent.OnDeath.AddListener(OnDeath);
        }

        private void Update()
        {
            if (!healthComponent.IsAlive || target == null) return;

            UpdateCooldowns();
            UpdateState();
            ExecuteState();
        }

        private void FixedUpdate()
        {
            if (!healthComponent.IsAlive) return;

            // Apply velocity
            rb.velocity = currentVelocity;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
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
            if (isAttacking) return; // Don't change state while attacking

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            switch (currentState)
            {
                case EnemyState.Idle:
                    if (distanceToTarget <= detectionRange)
                    {
                        ChangeState(EnemyState.Chase);
                    }
                    break;

                case EnemyState.Chase:
                    if (distanceToTarget <= attackRange && attackCooldownTimer <= 0)
                    {
                        ChangeState(EnemyState.Attack);
                    }
                    else if (distanceToTarget > loseTargetRange)
                    {
                        ChangeState(EnemyState.Idle);
                    }
                    break;

                case EnemyState.Attack:
                    // Return to chase after attack
                    if (!isAttacking)
                    {
                        ChangeState(EnemyState.Chase);
                    }
                    break;
            }
        }

        private void ExecuteState()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    StateIdle();
                    break;

                case EnemyState.Chase:
                    StateChase();
                    break;

                case EnemyState.Attack:
                    StateAttack();
                    break;
            }
        }

        private void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;

            if (debugMode)
            {
                Debug.Log($"[EnemyAI] {name}: {currentState} -> {newState}");
            }

            currentState = newState;
        }

        #endregion

        #region States

        private void StateIdle()
        {
            // Stop moving
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, acceleration * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool(HashIsMoving, false);
            }
        }

        private void StateChase()
        {
            if (target == null) return;

            // Move towards player
            Vector2 direction = (target.position - transform.position).normalized;
            Vector2 targetVelocity = direction * moveSpeed;

            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.deltaTime
            );

            // Update animation
            if (animator != null)
            {
                animator.SetBool(HashIsMoving, true);
            }

            // Flip sprite based on movement direction
            if (spriteRenderer != null && direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        private void StateAttack()
        {
            if (isAttacking) return;

            // Stop moving
            currentVelocity = Vector2.zero;

            // Perform attack
            StartCoroutine(AttackCoroutine());
        }

        #endregion

        #region Combat

        private System.Collections.IEnumerator AttackCoroutine()
        {
            isAttacking = true;
            attackCooldownTimer = attackCooldown;

            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger(HashAttack);
            }

            if (debugMode)
            {
                Debug.Log($"[EnemyAI] {name} attacking!");
            }

            // Wait for windup
            yield return new WaitForSeconds(attackWindup);

            // Perform hit check
            PerformAttackHit();

            // Wait for attack to finish
            yield return new WaitForSeconds(attackDuration - attackWindup);

            isAttacking = false;
        }

        private void PerformAttackHit()
        {
            // Check for player in attack range
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                attackPoint != null ? attackPoint.position : transform.position,
                attackRange,
                playerLayer
            );

            foreach (Collider2D hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    // Calculate damage
                    float damage = attackDamage;
                    if (stats != null)
                    {
                        damage = stats.CalculateDamage();
                    }

                    damageable.TakeDamage(damage, gameObject);

                    if (debugMode)
                    {
                        Debug.Log($"[EnemyAI] {name} hit player for {damage} damage!");
                    }
                }
            }
        }

        #endregion

        #region Health Events

        private void OnDamageTaken(float damage, GameObject source)
        {
            // Play hurt animation
            if (animator != null && !isAttacking)
            {
                animator.SetTrigger(HashHurt);
            }

            // Become aware of player if hit
            if (currentState == EnemyState.Idle && target != null)
            {
                ChangeState(EnemyState.Chase);
            }
        }

        private void OnDeath()
        {
            // Change state
            currentState = EnemyState.Dead;
            isAttacking = false;
            currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;

            // Play death animation
            if (animator != null)
            {
                animator.SetTrigger(HashDeath);
            }

            // Disable physics and AI
            rb.simulated = false;
            enabled = false;

            // Drop loot
            DropLoot();

            // Destroy after animation
            Destroy(gameObject, 2f);

            if (debugMode)
            {
                Debug.Log($"[EnemyAI] {name} died!");
            }
        }

        #endregion

        #region Loot

        private void DropLoot()
        {
            if (coinPrefab == null) return;

            // Drop coins in random directions
            for (int i = 0; i < coinsDropped; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
                Vector3 spawnPos = transform.position + (Vector3)randomOffset;

                GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

                // Add some physics to coins (bounce)
                Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
                if (coinRb != null)
                {
                    Vector2 randomForce = Random.insideUnitCircle * 2f;
                    coinRb.AddForce(randomForce, ForceMode2D.Impulse);
                }
            }
        }

        #endregion

        #region Utility

        private void UpdateCooldowns()
        {
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
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

            // Attack range
            Gizmos.color = Color.red;
            Vector3 attackPos = attackPoint != null ? attackPoint.position : transform.position;
            Gizmos.DrawWireSphere(attackPos, attackRange);

            // Line to target
            if (target != null && currentState != EnemyState.Idle)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }

        #endregion
    }

    /// <summary>
    /// Enemy AI states
    /// </summary>
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }
}
