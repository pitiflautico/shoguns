using UnityEngine;
using ShogunsLegacy.Data;

namespace ShogunsLegacy.Player
{
    /// <summary>
    /// Handles all player animations based on state and input
    /// Works with Unity Animator component
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float movementThreshold = 0.1f;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        private Animator animator;
        private HealthComponent healthComponent;

        // Animation state
        private bool isMoving = false;
        private bool isAttacking = false;
        private Vector2 moveDirection;

        // Animator parameter hashes (for performance)
        private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
        private static readonly int HashMoveX = Animator.StringToHash("MoveX");
        private static readonly int HashMoveY = Animator.StringToHash("MoveY");
        private static readonly int HashAttack = Animator.StringToHash("Attack");
        private static readonly int HashDash = Animator.StringToHash("Dash");
        private static readonly int HashHurt = Animator.StringToHash("Hurt");
        private static readonly int HashDeath = Animator.StringToHash("Death");
        private static readonly int HashAttackSpeed = Animator.StringToHash("AttackSpeed");

        #region Unity Lifecycle

        private void Awake()
        {
            animator = GetComponent<Animator>();
            healthComponent = GetComponent<HealthComponent>();
        }

        private void Start()
        {
            // Subscribe to health events
            if (healthComponent != null)
            {
                healthComponent.OnDamageTaken.AddListener(OnDamageTaken);
                healthComponent.OnDeath.AddListener(OnDeath);
            }
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

        #region Public Methods - Called by PlayerController

        /// <summary>
        /// Update movement state
        /// </summary>
        public void SetMoving(bool moving)
        {
            if (isMoving != moving)
            {
                isMoving = moving;
                animator.SetBool(HashIsMoving, moving);

                if (debugMode)
                {
                    Debug.Log($"[PlayerAnim] IsMoving: {moving}");
                }
            }
        }

        /// <summary>
        /// Update movement direction for blend tree
        /// </summary>
        public void SetMoveDirection(Vector2 direction)
        {
            if (direction.magnitude > movementThreshold)
            {
                moveDirection = direction.normalized;
                animator.SetFloat(HashMoveX, moveDirection.x);
                animator.SetFloat(HashMoveY, moveDirection.y);
            }
        }

        /// <summary>
        /// Trigger attack animation
        /// </summary>
        public void TriggerAttack()
        {
            animator.SetTrigger(HashAttack);
            isAttacking = true;

            if (debugMode)
            {
                Debug.Log("[PlayerAnim] Attack triggered");
            }
        }

        /// <summary>
        /// Trigger dash animation
        /// </summary>
        public void TriggerDash()
        {
            animator.SetTrigger(HashDash);

            if (debugMode)
            {
                Debug.Log("[PlayerAnim] Dash triggered");
            }
        }

        /// <summary>
        /// Set attack speed multiplier (for attack speed buffs)
        /// </summary>
        public void SetAttackSpeed(float speed)
        {
            animator.SetFloat(HashAttackSpeed, speed);
        }

        #endregion

        #region Animation Events - Called from Animation Clips

        /// <summary>
        /// Called from attack animation - when hitbox should be active
        /// </summary>
        public void OnAttackHitFrame()
        {
            // This will be caught by PlayerCombat
            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null)
            {
                combat.ActivateHitbox();
            }

            if (debugMode)
            {
                Debug.Log("[PlayerAnim] Attack hit frame!");
            }
        }

        /// <summary>
        /// Called when attack animation finishes
        /// </summary>
        public void OnAttackEnd()
        {
            isAttacking = false;

            if (debugMode)
            {
                Debug.Log("[PlayerAnim] Attack ended");
            }
        }

        /// <summary>
        /// Called during dash animation for effects
        /// </summary>
        public void OnDashEffect()
        {
            // Spawn dash VFX
            // This will be implemented in Stage 7
        }

        #endregion

        #region Health Events

        private void OnDamageTaken(float damage, GameObject source)
        {
            // Play hurt animation (doesn't interrupt other animations heavily)
            animator.SetTrigger(HashHurt);

            if (debugMode)
            {
                Debug.Log($"[PlayerAnim] Hurt! Damage: {damage}");
            }
        }

        private void OnDeath()
        {
            // Play death animation
            animator.SetTrigger(HashDeath);

            if (debugMode)
            {
                Debug.Log("[PlayerAnim] Death animation triggered");
            }
        }

        #endregion

        #region Utility

        /// <summary>
        /// Check if currently playing attack animation
        /// </summary>
        public bool IsAttacking()
        {
            return isAttacking;
        }

        /// <summary>
        /// Get current animation state info
        /// </summary>
        public AnimatorStateInfo GetCurrentStateInfo()
        {
            return animator.GetCurrentAnimatorStateInfo(0);
        }

        /// <summary>
        /// Check if animation is playing by name
        /// </summary>
        public bool IsPlayingAnimation(string animationName)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }

        #endregion
    }
}
