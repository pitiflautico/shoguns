using UnityEngine;
using ShogunsLegacy.Interfaces;
using ShogunsLegacy.Utils;

namespace ShogunsLegacy.Combat
{
    /// <summary>
    /// Generic projectile controller for all ranged attacks
    /// Handles movement, collision, damage, and lifetime
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileController : MonoBehaviour, IPooledObject
    {
        [Header("Movement")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private bool useGravity = false;
        [SerializeField] private float gravityScale = 0f;

        [Header("Damage")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private bool destroyOnHit = true;
        [SerializeField] private LayerMask targetLayers;

        [Header("Lifetime")]
        [SerializeField] private float maxLifetime = 5f;
        [SerializeField] private float maxDistance = 20f;

        [Header("Piercing")]
        [SerializeField] private bool canPierce = false;
        [SerializeField] private int maxPierceCount = 1;

        [Header("Homing (Optional)")]
        [SerializeField] private bool isHoming = false;
        [SerializeField] private float homingStrength = 5f;
        [SerializeField] private float homingDelay = 0.2f; // Time before homing activates

        [Header("VFX")]
        [SerializeField] private GameObject hitVFXPrefab;
        [SerializeField] private GameObject destroyVFXPrefab;
        [SerializeField] private TrailRenderer trail;

        [Header("Audio")]
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip destroySound;

        // Components
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Collider2D projectileCollider;

        // State
        private GameObject owner;
        private Vector2 direction;
        private float damage;
        private float spawnTime;
        private Vector3 spawnPosition;
        private int pierceCount = 0;
        private Transform homingTarget;
        private float homingTimer;

        #region Unity Lifecycle

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            projectileCollider = GetComponent<Collider2D>();

            // Configure Rigidbody2D
            rb.gravityScale = useGravity ? gravityScale : 0f;
        }

        private void Update()
        {
            CheckLifetime();
            CheckDistance();

            if (isHoming && homingTarget != null)
            {
                UpdateHoming();
            }
        }

        private void FixedUpdate()
        {
            // Rotate sprite to face movement direction
            if (rb.velocity.magnitude > 0.1f && spriteRenderer != null)
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize projectile with direction and optional damage override
        /// </summary>
        public void Initialize(Vector2 shootDirection, GameObject shooter, float damageOverride = -1f)
        {
            owner = shooter;
            direction = shootDirection.normalized;
            damage = damageOverride > 0 ? damageOverride : baseDamage;

            spawnTime = Time.time;
            spawnPosition = transform.position;
            pierceCount = 0;
            homingTimer = homingDelay;

            // Set velocity
            rb.velocity = direction * speed;

            // Reset trail
            if (trail != null)
            {
                trail.Clear();
            }
        }

        /// <summary>
        /// Set homing target
        /// </summary>
        public void SetHomingTarget(Transform target)
        {
            if (isHoming)
            {
                homingTarget = target;
            }
        }

        #endregion

        #region IPooledObject Implementation

        public void OnObjectSpawn()
        {
            // Reset state when spawned from pool
            gameObject.SetActive(true);
            pierceCount = 0;
            homingTimer = homingDelay;

            if (projectileCollider != null)
            {
                projectileCollider.enabled = true;
            }
        }

        #endregion

        #region Collision

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

            // Don't hit owner
            if (target == owner)
            {
                return;
            }

            // Try to damage
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(damage, owner);

                // Spawn hit VFX
                SpawnHitVFX(transform.position);

                // Play hit sound
                PlaySound(hitSound);

                // Check piercing
                if (canPierce && pierceCount < maxPierceCount)
                {
                    pierceCount++;
                    // Continue flying
                    return;
                }
            }

            // Destroy projectile
            if (destroyOnHit)
            {
                DestroyProjectile();
            }
        }

        #endregion

        #region Homing

        private void UpdateHoming()
        {
            homingTimer -= Time.deltaTime;
            if (homingTimer > 0) return; // Wait for delay

            if (homingTarget == null || !homingTarget.gameObject.activeInHierarchy)
            {
                homingTarget = null;
                return;
            }

            // Calculate direction to target
            Vector2 targetDirection = (homingTarget.position - transform.position).normalized;

            // Smoothly rotate towards target
            Vector2 newDirection = Vector2.Lerp(
                rb.velocity.normalized,
                targetDirection,
                homingStrength * Time.deltaTime
            );

            rb.velocity = newDirection * speed;
        }

        #endregion

        #region Lifetime Management

        private void CheckLifetime()
        {
            if (Time.time - spawnTime >= maxLifetime)
            {
                DestroyProjectile();
            }
        }

        private void CheckDistance()
        {
            float distanceTraveled = Vector3.Distance(transform.position, spawnPosition);
            if (distanceTraveled >= maxDistance)
            {
                DestroyProjectile();
            }
        }

        private void DestroyProjectile()
        {
            // Spawn destroy VFX
            if (destroyVFXPrefab != null)
            {
                Instantiate(destroyVFXPrefab, transform.position, Quaternion.identity);
            }

            // Play destroy sound
            PlaySound(destroySound);

            // Return to pool or destroy
            if (ObjectPooler.Instance != null)
            {
                // Try to return to pool (ObjectPooler will handle this)
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region VFX and Audio

        private void SpawnHitVFX(Vector3 position)
        {
            if (hitVFXPrefab != null)
            {
                GameObject vfx = Instantiate(hitVFXPrefab, position, Quaternion.identity);
                Destroy(vfx, 1f);
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set custom speed
        /// </summary>
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
        }

        /// <summary>
        /// Set custom damage
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
        /// Add force to projectile (for special effects)
        /// </summary>
        public void AddForce(Vector2 force)
        {
            if (rb != null)
            {
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            // Draw direction
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * 2f);

            // Draw homing target line
            if (isHoming && homingTarget != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, homingTarget.position);
            }
        }

        #endregion
    }
}
