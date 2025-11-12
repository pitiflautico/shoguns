using UnityEngine;
using Cinemachine;

namespace ShogunsLegacy.Utils
{
    /// <summary>
    /// Camera shake utility using Cinemachine Impulse
    /// Singleton pattern for easy access
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        [Header("Shake Presets")]
        [SerializeField] private ShakePreset lightShake = new ShakePreset(0.5f, 0.1f);
        [SerializeField] private ShakePreset mediumShake = new ShakePreset(1f, 0.2f);
        [SerializeField] private ShakePreset heavyShake = new ShakePreset(2f, 0.3f);

        [Header("Settings")]
        [SerializeField] private bool shakeEnabled = true;

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Get impulse source if not assigned
            if (impulseSource == null)
            {
                impulseSource = GetComponent<CinemachineImpulseSource>();

                if (impulseSource == null)
                {
                    impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
                }
            }

            // Get virtual camera if not assigned
            if (virtualCamera == null)
            {
                virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }
        }

        /// <summary>
        /// Shake with custom intensity and duration
        /// </summary>
        public void Shake(float intensity, float duration)
        {
            if (!shakeEnabled || impulseSource == null) return;

            impulseSource.GenerateImpulse(intensity);
        }

        /// <summary>
        /// Light shake (e.g., normal hit)
        /// </summary>
        public void ShakeLight()
        {
            Shake(lightShake.intensity, lightShake.duration);
        }

        /// <summary>
        /// Medium shake (e.g., critical hit, parry)
        /// </summary>
        public void ShakeMedium()
        {
            Shake(mediumShake.intensity, mediumShake.duration);
        }

        /// <summary>
        /// Heavy shake (e.g., boss attack, explosion)
        /// </summary>
        public void ShakeHeavy()
        {
            Shake(heavyShake.intensity, heavyShake.duration);
        }

        /// <summary>
        /// Shake at world position (impulse travels from that point)
        /// </summary>
        public void ShakeAtPosition(Vector3 position, float intensity)
        {
            if (!shakeEnabled || impulseSource == null) return;

            impulseSource.GenerateImpulseAtPositionWithVelocity(position, Vector3.zero);
        }

        /// <summary>
        /// Enable/disable shake
        /// </summary>
        public void SetShakeEnabled(bool enabled)
        {
            shakeEnabled = enabled;
        }

        /// <summary>
        /// Toggle shake on/off
        /// </summary>
        public void ToggleShake()
        {
            shakeEnabled = !shakeEnabled;
        }
    }

    /// <summary>
    /// Preset for camera shake parameters
    /// </summary>
    [System.Serializable]
    public class ShakePreset
    {
        public float intensity;
        public float duration;

        public ShakePreset(float intensity, float duration)
        {
            this.intensity = intensity;
            this.duration = duration;
        }
    }
}
