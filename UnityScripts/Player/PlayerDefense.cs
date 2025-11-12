using UnityEngine;
using UnityEngine.Events;
using ShogunsLegacy.Data;
using ShogunsLegacy.Managers;
using ShogunsLegacy.Combat;

namespace ShogunsLegacy.Player
{
    /// <summary>
    /// Handles player defensive mechanics: blocking, parrying, dodging
    /// Works alongside PlayerCombat for complete combat system
    /// </summary>
    public class PlayerDefense : MonoBehaviour
    {
        [Header("Block Settings")]
        [SerializeField] private bool canBlock = true;
        [SerializeField] private float blockDamageReduction = 0.8f; // 80% damage reduction
        [SerializeField] private float blockMovementPenalty = 0.5f; // 50% movement speed while blocking
        [SerializeField] private KeyCode blockKey = KeyCode.LeftShift;

        [Header("Parry Settings")]
        [SerializeField] private bool canParry = true;
        [SerializeField] private float parryWindow = 0.2f; // Perfect parry window in seconds
        [SerializeField] private float parrySuccessWindow = 0.3f; // Window to trigger parry after pressing block
        [SerializeField] private float parryCooldown = 1f;
        [SerializeField] private float parryStunDuration = 1.5f; // How long enemy is stunned

        [Header("Parry Projectile Reflection")]
        [SerializeField] private bool canReflectProjectiles = true;
        [SerializeField] private float reflectionDamageMultiplier = 1.5f;
        [SerializeField] private LayerMask projectileLayer;

        [Header("Dodge Settings (Future)")]
        [SerializeField] private bool perfectDodgeEnabled = false;
        [SerializeField] private float perfectDodgeWindow = 0.15f;

        [Header("VFX")]
        [SerializeField] private GameObject blockVFXPrefab;
        [SerializeField] private GameObject parryVFXPrefab;
        [SerializeField] private GameObject reflectVFXPrefab;
        [SerializeField] private Transform blockVFXSpawnPoint;

        [Header("Audio")]
        [SerializeField] private AudioClip blockSound;
        [SerializeField] private AudioClip parrySound;
        [SerializeField] private AudioClip reflectSound;

        [Header("Events")]
        public UnityEvent OnBlockStart;
        public UnityEvent OnBlockEnd;
        public UnityEvent OnParrySuccess;
        public UnityEvent<GameObject> OnProjectileReflected;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        // Components
        private HealthComponent healthComponent;
        private PlayerController playerController;
        private Animator animator;

        // State
        private bool isBlocking = false;
        private bool isInParryWindow = false;
        private float parryWindowTimer = 0f;
        private float parryCooldownTimer = 0f;
        private GameObject currentBlockVFX;

        // Original stats (to restore after blocking)
        private float originalDamageReduction;

        // Animation hashes
        private static readonly int HashBlock = Animator.StringToHash("Block");
        private static readonly int HashParry = Animator.StringToHash("Parry");

        #region Unity Lifecycle

        private void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            playerController = GetComponent<PlayerController>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Subscribe to damage events to check for parry
            if (healthComponent != null)
            {
                healthComponent.OnDamageTaken.AddListener(OnDamageReceived);
            }
        }

        private void Update()
        {
            UpdateCooldowns();
            HandleBlockInput();
            UpdateBlockState();
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
            {
                healthComponent.OnDamageTaken.RemoveListener(OnDamageReceived);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check for projectiles to reflect
            if (isInParryWindow && canReflectProjectiles)
            {
                CheckProjectileReflection(collision.gameObject);
            }
        }

        #endregion

        #region Block System

        private void HandleBlockInput()
        {
            if (!canBlock) return;

            // Start blocking
            if (Input.GetKeyDown(blockKey))
            {
                StartBlock();
            }

            // Stop blocking
            if (Input.GetKeyUp(blockKey))
            {
                StopBlock();
            }
        }

        private void StartBlock()
        {
            if (isBlocking) return;

            isBlocking = true;

            // Start parry window
            if (canParry && parryCooldownTimer <= 0)
            {
                isInParryWindow = true;
                parryWindowTimer = parrySuccessWindow;

                if (debugMode)
                {
                    Debug.Log("[PlayerDefense] Parry window active!");
                }
            }

            // Apply block damage reduction
            // Note: This assumes HealthComponent or a stats system can be modified at runtime
            // You may need to adjust based on your actual implementation

            // Reduce movement speed
            if (playerController != null)
            {
                // This would need to be implemented in PlayerController
                // playerController.SetSpeedMultiplier(blockMovementPenalty);
            }

            // Trigger animation
            if (animator != null)
            {
                animator.SetBool(HashBlock, true);
            }

            // Spawn block VFX
            if (blockVFXPrefab != null && blockVFXSpawnPoint != null)
            {
                currentBlockVFX = Instantiate(blockVFXPrefab, blockVFXSpawnPoint.position, Quaternion.identity, blockVFXSpawnPoint);
            }

            // Event
            OnBlockStart?.Invoke();

            if (debugMode)
            {
                Debug.Log("[PlayerDefense] Block started");
            }
        }

        private void StopBlock()
        {
            if (!isBlocking) return;

            isBlocking = false;
            isInParryWindow = false;

            // Restore movement speed
            if (playerController != null)
            {
                // playerController.SetSpeedMultiplier(1f);
            }

            // Stop animation
            if (animator != null)
            {
                animator.SetBool(HashBlock, false);
            }

            // Destroy block VFX
            if (currentBlockVFX != null)
            {
                Destroy(currentBlockVFX);
            }

            // Event
            OnBlockEnd?.Invoke();

            if (debugMode)
            {
                Debug.Log("[PlayerDefense] Block ended");
            }
        }

        private void UpdateBlockState()
        {
            // Update parry window
            if (isInParryWindow)
            {
                parryWindowTimer -= Time.deltaTime;
                if (parryWindowTimer <= 0)
                {
                    isInParryWindow = false;

                    if (debugMode)
                    {
                        Debug.Log("[PlayerDefense] Parry window closed");
                    }
                }
            }
        }

        #endregion

        #region Parry System

        private void OnDamageReceived(float damage, GameObject source)
        {
            // Check if we're in parry window
            if (isInParryWindow && canParry)
            {
                ExecuteParry(source);
            }
            else if (isBlocking)
            {
                // Just blocking, reduce damage
                // Note: Damage reduction is already applied via HealthComponent
                // This is just for VFX/audio feedback
                PlayBlockEffects();
            }
        }

        private void ExecuteParry(GameObject attacker)
        {
            isInParryWindow = false;
            parryCooldownTimer = parryCooldown;

            // Play parry animation
            if (animator != null)
            {
                animator.SetTrigger(HashParry);
            }

            // Spawn parry VFX
            if (parryVFXPrefab != null)
            {
                Vector3 spawnPos = blockVFXSpawnPoint != null ? blockVFXSpawnPoint.position : transform.position;
                GameObject vfx = Instantiate(parryVFXPrefab, spawnPos, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            // Play parry sound
            if (parrySound != null)
            {
                AudioSource.PlayClipAtPoint(parrySound, transform.position);
            }

            // Stun attacker
            StunEnemy(attacker);

            // Event
            OnParrySuccess?.Invoke();

            if (debugMode)
            {
                Debug.Log($"[PlayerDefense] PARRY SUCCESS against {attacker.name}!");
            }
        }

        private void StunEnemy(GameObject enemy)
        {
            if (enemy == null) return;

            // Try to stun enemy AI
            var enemyAI = enemy.GetComponent<ShogunsLegacy.Enemy.EnemyAI>();
            if (enemyAI != null)
            {
                // This would need to be implemented in EnemyAI
                // enemyAI.Stun(parryStunDuration);
            }

            // Apply knockback
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDir * 10f, ForceMode2D.Impulse);
            }
        }

        #endregion

        #region Projectile Reflection

        private void CheckProjectileReflection(GameObject obj)
        {
            // Check if it's a projectile
            if (((1 << obj.layer) & projectileLayer) == 0)
                return;

            ProjectileController projectile = obj.GetComponent<ProjectileController>();
            if (projectile != null)
            {
                ReflectProjectile(projectile);
            }
        }

        private void ReflectProjectile(ProjectileController projectile)
        {
            // Reflect projectile back
            Vector2 reflectDirection = -projectile.transform.right; // Reverse direction

            // Reinitialize projectile with player as owner
            float newDamage = projectile.GetComponent<DamageDealer>() != null ?
                              reflectionDamageMultiplier * 10f : // Default damage if no DamageDealer
                              0f;

            projectile.Initialize(reflectDirection, gameObject, newDamage);

            // Change target layers to hit enemies
            projectile.SetTargetLayers(LayerMask.GetMask("Enemy"));

            // Spawn reflect VFX
            if (reflectVFXPrefab != null)
            {
                GameObject vfx = Instantiate(reflectVFXPrefab, projectile.transform.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            // Play reflect sound
            if (reflectSound != null)
            {
                AudioSource.PlayClipAtPoint(reflectSound, transform.position);
            }

            // Event
            OnProjectileReflected?.Invoke(projectile.gameObject);

            // Success parry (reset parry window)
            isInParryWindow = false;
            parryCooldownTimer = parryCooldown;

            if (debugMode)
            {
                Debug.Log("[PlayerDefense] Reflected projectile!");
            }
        }

        #endregion

        #region VFX and Audio

        private void PlayBlockEffects()
        {
            // Play block sound
            if (blockSound != null)
            {
                AudioSource.PlayClipAtPoint(blockSound, transform.position);
            }

            // Flash block VFX
            if (currentBlockVFX != null)
            {
                // Could add a flash/pulse effect here
            }
        }

        #endregion

        #region Utility

        private void UpdateCooldowns()
        {
            if (parryCooldownTimer > 0)
            {
                parryCooldownTimer -= Time.deltaTime;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if currently blocking
        /// </summary>
        public bool IsBlocking()
        {
            return isBlocking;
        }

        /// <summary>
        /// Check if in parry window
        /// </summary>
        public bool IsInParryWindow()
        {
            return isInParryWindow;
        }

        /// <summary>
        /// Check if parry is on cooldown
        /// </summary>
        public bool IsParryOnCooldown()
        {
            return parryCooldownTimer > 0;
        }

        /// <summary>
        /// Get current block damage reduction
        /// </summary>
        public float GetBlockDamageReduction()
        {
            return isBlocking ? blockDamageReduction : 0f;
        }

        /// <summary>
        /// Force stop blocking (useful for interrupts)
        /// </summary>
        public void ForceStopBlock()
        {
            if (isBlocking)
            {
                StopBlock();
            }
        }

        #endregion
    }
}
