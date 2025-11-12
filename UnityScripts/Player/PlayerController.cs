using UnityEngine;
using ShogunsLegacy.Data;
using ShogunsLegacy.Managers;

namespace ShogunsLegacy.Player
{
    /// <summary>
    /// Main player controller - handles movement, rotation, and physics
    /// Reads input from InputManager singleton
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(HealthComponent))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameStats stats;
        [SerializeField] private Transform weaponPivot; // Pivot point for weapon/aim direction
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 50f; // How fast we reach max speed
        [SerializeField] private float deceleration = 50f; // How fast we stop

        [Header("Dash Settings")]
        [SerializeField] private bool canDash = true;
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private int dashesPerCooldown = 1;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        // Components
        private Rigidbody2D rb;
        private HealthComponent healthComponent;
        private PlayerCombat playerCombat;
        private PlayerAnimationController animController;

        // State
        private Vector2 moveInput;
        private Vector2 currentVelocity;
        private Vector2 aimDirection;
        private bool isDashing = false;
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private int dashesRemaining;

        // Properties
        public Vector2 AimDirection => aimDirection;
        public bool IsDashing => isDashing;
        public bool IsMoving => currentVelocity.magnitude > 0.1f;
        public float CurrentSpeed => currentVelocity.magnitude;

        #region Unity Lifecycle

        private void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>();
            healthComponent = GetComponent<HealthComponent>();
            playerCombat = GetComponent<PlayerCombat>();
            animController = GetComponent<PlayerAnimationController>();

            // Configure Rigidbody2D
            rb.gravityScale = 0f; // Top-down, no gravity
            rb.drag = 0f; // We handle deceleration manually
            rb.freezeRotation = true; // Don't rotate from physics

            // Initialize dashes
            dashesRemaining = dashesPerCooldown;
        }

        private void Start()
        {
            // Apply stats if available
            if (stats != null)
            {
                moveSpeed = stats.moveSpeed;
                canDash = stats.canDash;
                dashCooldown = stats.dashCooldown;
                dashSpeed = stats.dashDistance / dashDuration; // Calculate speed from distance
            }

            // Subscribe to health events
            if (healthComponent != null)
            {
                healthComponent.OnDeath.AddListener(OnPlayerDeath);
            }
        }

        private void Update()
        {
            if (!healthComponent.IsAlive) return;

            // Get input from InputManager
            if (InputManager.Instance != null)
            {
                moveInput = InputManager.Instance.MoveInput;

                // Update aim direction based on mouse position
                Vector3 mousePos = InputManager.Instance.MouseWorldPosition;
                aimDirection = (mousePos - transform.position).normalized;

                // Check for dash input
                if (InputManager.Instance.DashPressed && CanDash())
                {
                    StartDash();
                }
            }

            // Update timers
            UpdateDash();
            UpdateCooldowns();

            // Update weapon pivot rotation
            UpdateWeaponRotation();

            // Flip sprite based on aim direction
            UpdateSpriteFlip();
        }

        private void FixedUpdate()
        {
            if (!healthComponent.IsAlive) return;

            if (isDashing)
            {
                HandleDashMovement();
            }
            else
            {
                HandleNormalMovement();
            }
        }

        #endregion

        #region Movement

        private void HandleNormalMovement()
        {
            // Calculate target velocity
            Vector2 targetVelocity = moveInput.normalized * moveSpeed;

            // Smoothly interpolate to target velocity
            if (moveInput.magnitude > 0.1f)
            {
                // Accelerating
                currentVelocity = Vector2.MoveTowards(
                    currentVelocity,
                    targetVelocity,
                    acceleration * Time.fixedDeltaTime
                );
            }
            else
            {
                // Decelerating
                currentVelocity = Vector2.MoveTowards(
                    currentVelocity,
                    Vector2.zero,
                    deceleration * Time.fixedDeltaTime
                );
            }

            // Apply velocity
            rb.velocity = currentVelocity;

            // Notify animation controller
            if (animController != null)
            {
                animController.SetMoving(IsMoving);
                animController.SetMoveDirection(currentVelocity.normalized);
            }
        }

        private void HandleDashMovement()
        {
            // Dash in the direction of movement input, or aim direction if not moving
            Vector2 dashDir = moveInput.magnitude > 0.1f ? moveInput.normalized : aimDirection;
            rb.velocity = dashDir * dashSpeed;
        }

        #endregion

        #region Dash System

        private bool CanDash()
        {
            return canDash && !isDashing && dashesRemaining > 0;
        }

        private void StartDash()
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashesRemaining--;

            // Make invulnerable during dash
            if (healthComponent != null)
            {
                healthComponent.SetInvulnerable(dashDuration);
            }

            // Notify animation
            if (animController != null)
            {
                animController.TriggerDash();
            }

            if (debugMode)
            {
                Debug.Log($"[Player] Dashing! Remaining: {dashesRemaining}");
            }
        }

        private void UpdateDash()
        {
            if (isDashing)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                }
            }
        }

        private void UpdateCooldowns()
        {
            // Dash cooldown
            if (dashesRemaining < dashesPerCooldown)
            {
                dashCooldownTimer -= Time.deltaTime;
                if (dashCooldownTimer <= 0)
                {
                    dashesRemaining = dashesPerCooldown;
                    dashCooldownTimer = dashCooldown;
                }
            }
        }

        #endregion

        #region Aiming and Rotation

        private void UpdateWeaponRotation()
        {
            if (weaponPivot != null && aimDirection.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                weaponPivot.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void UpdateSpriteFlip()
        {
            if (spriteRenderer != null && aimDirection.magnitude > 0.1f)
            {
                // Flip sprite based on aim direction
                spriteRenderer.flipX = aimDirection.x < 0;
            }
        }

        #endregion

        #region Health Events

        private void OnPlayerDeath()
        {
            // Stop movement
            rb.velocity = Vector2.zero;
            currentVelocity = Vector2.zero;

            // Disable controls
            enabled = false;

            // Notify game manager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.GameOver);
            }

            if (debugMode)
            {
                Debug.Log("[Player] Player died!");
            }

            // Animation handled by HealthComponent event in PlayerAnimationController
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Force stop movement (useful for cutscenes, etc.)
        /// </summary>
        public void StopMovement()
        {
            rb.velocity = Vector2.zero;
            currentVelocity = Vector2.zero;
            moveInput = Vector2.zero;
        }

        /// <summary>
        /// Teleport player to position
        /// </summary>
        public void Teleport(Vector3 position)
        {
            transform.position = position;
            StopMovement();
        }

        /// <summary>
        /// Apply knockback force
        /// </summary>
        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (isDashing) return; // Immune to knockback while dashing

            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Get stats (for combat system to read)
        /// </summary>
        public GameStats GetStats()
        {
            return stats;
        }

        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            if (!debugMode) return;

            // Draw aim direction
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)aimDirection * 2f);

            // Draw velocity
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentVelocity);
        }

        #endregion
    }
}
