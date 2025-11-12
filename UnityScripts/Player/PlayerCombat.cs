using UnityEngine;
using System.Collections;
using ShogunsLegacy.Data;
using ShogunsLegacy.Managers;
using ShogunsLegacy.Interfaces;

namespace ShogunsLegacy.Player
{
    /// <summary>
    /// Handles player combat - melee attacks, projectiles, combos
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameStats stats;
        [SerializeField] private Transform attackPoint; // Where the hitbox spawns
        [SerializeField] private LayerMask enemyLayers; // What can be damaged

        [Header("Melee Attack Settings")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackDuration = 0.3f; // How long attack animation lasts
        [SerializeField] private float hitboxActiveTime = 0.1f; // How long hitbox is active

        [Header("Combo System")]
        [SerializeField] private bool comboEnabled = true;
        [SerializeField] private int maxComboCount = 3;
        [SerializeField] private float comboWindow = 1.5f; // Time to continue combo
        [SerializeField] private float comboMultiplier = 1.5f; // Damage multiplier on final combo hit

        [Header("VFX")]
        [SerializeField] private GameObject slashVFXPrefab;
        [SerializeField] private GameObject hitVFXPrefab;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool showGizmos = true;

        // Components
        private PlayerController playerController;
        private PlayerAnimationController animController;
        private HealthComponent healthComponent;

        // State
        private bool canAttack = true;
        private bool isAttacking = false;
        private float attackCooldownTimer = 0f;

        // Combo state
        private int currentComboCount = 0;
        private float comboTimer = 0f;

        // Projectile state (for Stage 2)
        private float projectileCooldownTimer = 0f;

        #region Unity Lifecycle

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            animController = GetComponent<PlayerAnimationController>();
            healthComponent = GetComponent<HealthComponent>();
        }

        private void Start()
        {
            // Apply stats
            if (stats != null)
            {
                attackCooldown = 1f / stats.attackSpeed; // Convert attack speed to cooldown
            }
        }

        private void Update()
        {
            if (!healthComponent.IsAlive) return;

            // Update timers
            UpdateCooldowns();
            UpdateCombo();

            // Check for attack input
            if (InputManager.Instance != null)
            {
                if (InputManager.Instance.AttackPressed && canAttack)
                {
                    PerformMeleeAttack();
                }

                // Secondary attack (projectiles) - will be implemented in Stage 2
                if (InputManager.Instance.SecondaryAttackPressed)
                {
                    // TODO: Stage 2 - Projectile attack
                }
            }
        }

        #endregion

        #region Melee Attack

        private void PerformMeleeAttack()
        {
            if (!canAttack || isAttacking) return;

            // Start attack
            isAttacking = true;
            canAttack = false;
            attackCooldownTimer = attackCooldown;

            // Update combo
            if (comboEnabled)
            {
                currentComboCount++;
                comboTimer = comboWindow;

                if (currentComboCount > maxComboCount)
                {
                    currentComboCount = 1; // Reset combo
                }
            }

            // Trigger animation
            if (animController != null)
            {
                animController.TriggerAttack();
            }

            // Spawn VFX
            SpawnSlashVFX();

            // Start attack coroutine
            StartCoroutine(AttackCoroutine());

            if (debugMode)
            {
                Debug.Log($"[PlayerCombat] Attack! Combo: {currentComboCount}/{maxComboCount}");
            }
        }

        private IEnumerator AttackCoroutine()
        {
            // Wait a bit before activating hitbox (sync with animation)
            yield return new WaitForSeconds(attackDuration * 0.3f);

            // Hitbox is activated by animation event in PlayerAnimationController
            // But we can also activate it here as fallback
            // ActivateHitbox();

            // Wait for attack to finish
            yield return new WaitForSeconds(attackDuration * 0.7f);

            isAttacking = false;
        }

        /// <summary>
        /// Called by animation event when hitbox should be active
        /// </summary>
        public void ActivateHitbox()
        {
            // Detect enemies in range
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayers
            );

            // Calculate damage
            float damage = CalculateDamage();

            // Damage all hit enemies
            foreach (Collider2D enemy in hitEnemies)
            {
                IDamageable damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    damageable.TakeDamage(damage, gameObject);

                    // Spawn hit VFX
                    SpawnHitVFX(enemy.transform.position);

                    // Apply knockback
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                        enemyRb.AddForce(knockbackDir * 3f, ForceMode2D.Impulse);
                    }

                    if (debugMode)
                    {
                        Debug.Log($"[PlayerCombat] Hit {enemy.name} for {damage} damage!");
                    }
                }
            }

            // Screen shake on hit (Stage 7)
            if (hitEnemies.Length > 0)
            {
                // TODO: CameraShake
            }
        }

        private float CalculateDamage()
        {
            float baseDamage = stats != null ? stats.baseDamage : 10f;

            // Apply combo multiplier on last hit
            if (comboEnabled && currentComboCount == maxComboCount)
            {
                baseDamage *= comboMultiplier;

                if (debugMode)
                {
                    Debug.Log($"[PlayerCombat] COMBO FINISHER! Damage: {baseDamage}");
                }
            }

            // Calculate critical hit
            if (stats != null)
            {
                if (Random.value < stats.criticalChance)
                {
                    baseDamage *= stats.criticalMultiplier;

                    if (debugMode)
                    {
                        Debug.Log($"[PlayerCombat] CRITICAL HIT! Damage: {baseDamage}");
                    }
                }
            }

            return baseDamage;
        }

        #endregion

        #region VFX

        private void SpawnSlashVFX()
        {
            if (slashVFXPrefab == null) return;

            Vector3 spawnPos = attackPoint != null ? attackPoint.position : transform.position;
            Quaternion rotation = Quaternion.identity;

            // Rotate VFX to face aim direction
            if (playerController != null)
            {
                Vector2 aimDir = playerController.AimDirection;
                float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
                rotation = Quaternion.Euler(0, 0, angle);
            }

            GameObject vfx = Instantiate(slashVFXPrefab, spawnPos, rotation);
            Destroy(vfx, 1f); // Auto-destroy after 1 second
        }

        private void SpawnHitVFX(Vector3 position)
        {
            if (hitVFXPrefab == null) return;

            GameObject vfx = Instantiate(hitVFXPrefab, position, Quaternion.identity);
            Destroy(vfx, 0.5f);
        }

        #endregion

        #region Cooldowns and Timers

        private void UpdateCooldowns()
        {
            // Attack cooldown
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
                if (attackCooldownTimer <= 0)
                {
                    canAttack = true;
                }
            }

            // Projectile cooldown (Stage 2)
            if (projectileCooldownTimer > 0)
            {
                projectileCooldownTimer -= Time.deltaTime;
            }
        }

        private void UpdateCombo()
        {
            if (!comboEnabled) return;

            // Combo timer
            if (comboTimer > 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0)
                {
                    // Combo expired
                    currentComboCount = 0;

                    if (debugMode)
                    {
                        Debug.Log("[PlayerCombat] Combo reset");
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if player can attack
        /// </summary>
        public bool CanAttack()
        {
            return canAttack && !isAttacking && healthComponent.IsAlive;
        }

        /// <summary>
        /// Get current combo count
        /// </summary>
        public int GetComboCount()
        {
            return currentComboCount;
        }

        /// <summary>
        /// Reset combo manually
        /// </summary>
        public void ResetCombo()
        {
            currentComboCount = 0;
            comboTimer = 0f;
        }

        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            if (!showGizmos || attackPoint == null) return;

            // Draw attack range
            Gizmos.color = isAttacking ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        #endregion
    }
}
