using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShogunsLegacy.Data;

namespace ShogunsLegacy.UI
{
    /// <summary>
    /// Health bar UI component - displays health with smooth transitions
    /// Can be used for player or enemy health bars
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthComponent targetHealth;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Settings")]
        [SerializeField] private bool smoothTransition = true;
        [SerializeField] private float transitionSpeed = 5f;
        [SerializeField] private bool showNumbers = true;
        [SerializeField] private bool hideWhenFull = false;
        [SerializeField] private bool followTarget = false; // For world-space enemy health bars

        [Header("Colors")]
        [SerializeField] private Gradient healthGradient;
        [SerializeField] private bool useGradient = true;
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color lowHealthColor = Color.red;

        [Header("Damage Flash")]
        [SerializeField] private bool flashOnDamage = true;
        [SerializeField] private float flashDuration = 0.2f;
        [SerializeField] private Color flashColor = Color.white;

        private float targetFillAmount = 1f;
        private float currentFillAmount = 1f;
        private bool isFlashing = false;
        private float flashTimer = 0f;
        private Color originalColor;
        private Canvas canvas;

        #region Unity Lifecycle

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();

            if (fillImage != null)
            {
                originalColor = fillImage.color;
            }
        }

        private void Start()
        {
            if (targetHealth != null)
            {
                // Subscribe to health events
                targetHealth.OnHealthChanged.AddListener(OnHealthChanged);
                targetHealth.OnDamageTaken.AddListener(OnDamageTaken);
                targetHealth.OnDeath.AddListener(OnDeath);

                // Initialize
                UpdateHealthBar(targetHealth.GetHealthNormalized(), true);
            }

            // Hide if configured
            if (hideWhenFull && targetFillAmount >= 1f)
            {
                canvas.enabled = false;
            }
        }

        private void Update()
        {
            // Smooth transition
            if (smoothTransition && Mathf.Abs(currentFillAmount - targetFillAmount) > 0.01f)
            {
                currentFillAmount = Mathf.Lerp(
                    currentFillAmount,
                    targetFillAmount,
                    Time.deltaTime * transitionSpeed
                );

                if (fillImage != null)
                {
                    fillImage.fillAmount = currentFillAmount;
                }
            }

            // Flash effect
            if (isFlashing)
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0)
                {
                    isFlashing = false;
                    if (fillImage != null)
                    {
                        fillImage.color = originalColor;
                    }
                }
            }

            // Follow target (for world-space enemy health bars)
            if (followTarget && targetHealth != null)
            {
                Vector3 targetPos = targetHealth.transform.position;
                targetPos.y += 1f; // Offset above enemy
                transform.position = targetPos;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (targetHealth != null)
            {
                targetHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
                targetHealth.OnDamageTaken.RemoveListener(OnDamageTaken);
                targetHealth.OnDeath.RemoveListener(OnDeath);
            }
        }

        #endregion

        #region Health Events

        private void OnHealthChanged(float currentHealth)
        {
            if (targetHealth == null) return;

            float healthPercent = targetHealth.GetHealthNormalized();
            UpdateHealthBar(healthPercent);

            // Show bar if it was hidden
            if (hideWhenFull && healthPercent < 1f && canvas != null)
            {
                canvas.enabled = true;
            }
        }

        private void OnDamageTaken(float damage, GameObject source)
        {
            // Flash effect
            if (flashOnDamage && fillImage != null)
            {
                isFlashing = true;
                flashTimer = flashDuration;
                fillImage.color = flashColor;
            }
        }

        private void OnDeath()
        {
            // Optionally hide or show empty bar
            // You can add death animation here
        }

        #endregion

        #region Update UI

        private void UpdateHealthBar(float healthPercent, bool instant = false)
        {
            targetFillAmount = Mathf.Clamp01(healthPercent);

            if (instant || !smoothTransition)
            {
                currentFillAmount = targetFillAmount;
                if (fillImage != null)
                {
                    fillImage.fillAmount = currentFillAmount;
                }
            }

            // Update color based on health
            UpdateColor(targetFillAmount);

            // Update text
            UpdateText();
        }

        private void UpdateColor(float healthPercent)
        {
            if (fillImage == null) return;

            if (useGradient && healthGradient != null)
            {
                originalColor = healthGradient.Evaluate(healthPercent);
            }
            else
            {
                originalColor = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
            }

            if (!isFlashing)
            {
                fillImage.color = originalColor;
            }
        }

        private void UpdateText()
        {
            if (healthText == null || !showNumbers || targetHealth == null) return;

            healthText.text = $"{Mathf.CeilToInt(targetHealth.CurrentHealth)}/{Mathf.CeilToInt(targetHealth.MaxHealth)}";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set target health component dynamically
        /// </summary>
        public void SetTargetHealth(HealthComponent health)
        {
            // Unsubscribe from old target
            if (targetHealth != null)
            {
                targetHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
                targetHealth.OnDamageTaken.RemoveListener(OnDamageTaken);
                targetHealth.OnDeath.RemoveListener(OnDeath);
            }

            targetHealth = health;

            // Subscribe to new target
            if (targetHealth != null)
            {
                targetHealth.OnHealthChanged.AddListener(OnHealthChanged);
                targetHealth.OnDamageTaken.AddListener(OnDamageTaken);
                targetHealth.OnDeath.AddListener(OnDeath);

                UpdateHealthBar(targetHealth.GetHealthNormalized(), true);
            }
        }

        /// <summary>
        /// Show or hide health bar
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (canvas != null)
            {
                canvas.enabled = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        #endregion
    }
}
