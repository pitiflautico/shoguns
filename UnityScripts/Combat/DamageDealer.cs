using UnityEngine;
using ShogunsLegacy.Interfaces;

namespace ShogunsLegacy.Combat
{
    /// <summary>
    /// Component that deals damage to IDamageable objects on contact
    /// Attach to weapons, projectiles, enemy attacks, etc.
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private bool useOwnerDamage = false; // Use damage from owner's stats instead

        [Header("Behavior")]
        [SerializeField] private bool destroyOnHit = false;
        [SerializeField] private float lifetime = -1f; // -1 = infinite
        [SerializeField] private LayerMask targetLayers; // What can this damage?

        [Header("Knockback")]
        [SerializeField] private bool applyKnockback = false;
        [SerializeField] private float knockbackForce = 5f;

        private GameObject owner; // Who created this damage dealer?
        private float spawnTime;

        private void Start()
        {
            spawnTime = Time.time;
        }

        private void Update()
        {
            // Check lifetime
            if (lifetime > 0 && Time.time - spawnTime >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ProcessCollision(collision.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcessCollision(collision.gameObject);
        }

        private void ProcessCollision(GameObject target)
        {
            // Check if target is in valid layers
            if (((1 << target.layer) & targetLayers) == 0)
            {
                return;
            }

            // Don't damage owner
            if (target == owner)
            {
                return;
            }

            // Try to damage
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                float finalDamage = useOwnerDamage ? GetOwnerDamage() : damage;
                damageable.TakeDamage(finalDamage, owner);

                // Apply knockback
                if (applyKnockback)
                {
                    Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
                    if (targetRb != null)
                    {
                        Vector2 knockbackDir = (target.transform.position - transform.position).normalized;
                        targetRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                    }
                }

                // Destroy this object if configured
                if (destroyOnHit)
                {
                    Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// Set the owner of this damage dealer (who created it)
        /// </summary>
        public void SetOwner(GameObject newOwner)
        {
            owner = newOwner;
        }

        /// <summary>
        /// Set custom damage value
        /// </summary>
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }

        /// <summary>
        /// Set target layers
        /// </summary>
        public void SetTargetLayers(LayerMask layers)
        {
            targetLayers = layers;
        }

        /// <summary>
        /// Get damage from owner's stats if available
        /// </summary>
        private float GetOwnerDamage()
        {
            if (owner == null) return damage;

            // Try to get stats from owner
            // This will be implemented when we create the player/enemy controllers
            // For now, return base damage
            return damage;
        }
    }
}
