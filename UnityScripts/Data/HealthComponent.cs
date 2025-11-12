using UnityEngine;
using UnityEngine.Events;
using ShogunsLegacy.Interfaces;

namespace ShogunsLegacy.Data
{
    /// <summary>
    /// Generic health component that can be attached to any GameObject
    /// Implements IDamageable interface
    /// </summary>
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Invulnerability")]
        [SerializeField] private bool hasInvulnerabilityFrames = true;
        [SerializeField] private float invulnerabilityDuration = 0.5f;
        private float invulnerabilityTimer = 0f;

        [Header("Events")]
        public UnityEvent<float> OnHealthChanged;
        public UnityEvent<float, GameObject> OnDamageTaken;
        public UnityEvent OnDeath;
        public UnityEvent OnHealed;

        #region IDamageable Implementation
        public bool IsAlive => currentHealth > 0;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        #endregion

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if (invulnerabilityTimer > 0)
            {
                invulnerabilityTimer -= Time.deltaTime;
            }
        }

        public void TakeDamage(float damage, GameObject damageSource = null)
        {
            if (!IsAlive) return;

            // Check invulnerability frames
            if (hasInvulnerabilityFrames && invulnerabilityTimer > 0)
            {
                return; // Still invulnerable
            }

            // Apply damage
            currentHealth = Mathf.Max(0, currentHealth - damage);

            // Trigger events
            OnHealthChanged?.Invoke(currentHealth);
            OnDamageTaken?.Invoke(damage, damageSource);

            // Start invulnerability
            if (hasInvulnerabilityFrames)
            {
                invulnerabilityTimer = invulnerabilityDuration;
            }

            // Check death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth);
            OnHealed?.Invoke();
        }

        public void SetMaxHealth(float newMax, bool healToFull = false)
        {
            maxHealth = newMax;
            if (healToFull)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = Mathf.Min(currentHealth, maxHealth);
            }
            OnHealthChanged?.Invoke(currentHealth);
        }

        private void Die()
        {
            OnDeath?.Invoke();
        }

        /// <summary>
        /// Get current health as normalized value (0-1)
        /// </summary>
        public float GetHealthNormalized()
        {
            return currentHealth / maxHealth;
        }

        /// <summary>
        /// Force invulnerability for a duration (useful for special moves)
        /// </summary>
        public void SetInvulnerable(float duration)
        {
            invulnerabilityTimer = duration;
        }

        /// <summary>
        /// Check if currently invulnerable
        /// </summary>
        public bool IsInvulnerable()
        {
            return hasInvulnerabilityFrames && invulnerabilityTimer > 0;
        }
    }
}
