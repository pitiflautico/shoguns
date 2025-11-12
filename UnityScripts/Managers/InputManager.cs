using UnityEngine;
using UnityEngine.InputSystem;

namespace ShogunsLegacy.Managers
{
    /// <summary>
    /// Centralized input manager using Unity's new Input System
    /// Singleton pattern for easy access from any script
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        // Input values
        public Vector2 MoveInput { get; private set; }
        public Vector2 AimInput { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool SecondaryAttackPressed { get; private set; }
        public bool DashPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool PausePressed { get; private set; }

        // Mouse position in world space
        public Vector3 MouseWorldPosition { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool enableInput = true;

        private Camera mainCamera;

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!enableInput) return;

            // Update mouse world position
            if (mainCamera != null)
            {
                MouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                MouseWorldPosition.z = 0;
            }

            // Reset one-frame inputs
            AttackPressed = false;
            SecondaryAttackPressed = false;
            DashPressed = false;
            InteractPressed = false;
            PausePressed = false;
        }

        #endregion

        #region Input Callbacks (Connect these to Input Actions)

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!enableInput) return;
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (!enableInput) return;
            AimInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!enableInput) return;
            if (context.performed)
            {
                AttackPressed = true;
            }
        }

        public void OnSecondaryAttack(InputAction.CallbackContext context)
        {
            if (!enableInput) return;
            if (context.performed)
            {
                SecondaryAttackPressed = true;
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (!enableInput) return;
            if (context.performed)
            {
                DashPressed = true;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!enableInput) return;
            if (context.performed)
            {
                InteractPressed = true;
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PausePressed = true;
            }
        }

        #endregion

        #region Public Methods

        public void EnableInput()
        {
            enableInput = true;
        }

        public void DisableInput()
        {
            enableInput = false;
            MoveInput = Vector2.zero;
            AimInput = Vector2.zero;
        }

        /// <summary>
        /// Get direction from player to mouse in world space
        /// </summary>
        public Vector2 GetAimDirection(Vector3 fromPosition)
        {
            Vector2 direction = (MouseWorldPosition - fromPosition).normalized;
            return direction;
        }

        #endregion
    }
}
