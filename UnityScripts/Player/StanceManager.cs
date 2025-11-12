using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using ShogunsLegacy.Data;
using ShogunsLegacy.Managers;

namespace ShogunsLegacy.Player
{
    /// <summary>
    /// Manages combat stances for the player
    /// Allows switching between different combat styles (Water, Earth, Wind, etc.)
    /// </summary>
    public class StanceManager : MonoBehaviour
    {
        [Header("Available Stances")]
        [SerializeField] private List<CombatStance> availableStances = new List<CombatStance>();
        [SerializeField] private CombatStance defaultStance;

        [Header("Settings")]
        [SerializeField] private bool canSwitchDuringCombat = true;
        [SerializeField] private float switchCooldown = 0.5f;
        [SerializeField] private KeyCode cycleStanceKey = KeyCode.Q;

        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerCombat playerCombat;

        [Header("VFX")]
        [SerializeField] private Transform auraVFXSpawnPoint;
        [SerializeField] private float stanceSwitchVFXDuration = 1f;

        [Header("Events")]
        public UnityEvent<CombatStance> OnStanceChanged;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        // State
        private CombatStance currentStance;
        private int currentStanceIndex = 0;
        private float switchCooldownTimer = 0f;
        private GameObject currentAuraVFX;
        private GameStats baseStats;
        private GameStats modifiedStats;

        #region Unity Lifecycle

        private void Awake()
        {
            // Get components if not assigned
            if (playerController == null)
                playerController = GetComponent<PlayerController>();

            if (playerCombat == null)
                playerCombat = GetComponent<PlayerCombat>();

            // Store base stats
            if (playerController != null)
            {
                baseStats = playerController.GetStats();
            }
        }

        private void Start()
        {
            // Initialize with default stance
            if (defaultStance != null)
            {
                SetStance(defaultStance, true);
            }
            else if (availableStances.Count > 0)
            {
                SetStance(availableStances[0], true);
            }
        }

        private void Update()
        {
            UpdateCooldown();
            CheckInput();
        }

        #endregion

        #region Input

        private void CheckInput()
        {
            // Check for stance switch input
            if (Input.GetKeyDown(cycleStanceKey) && CanSwitchStance())
            {
                CycleToNextStance();
            }

            // Alternative: number keys for direct stance selection
            if (availableStances.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(availableStances.Count, 9); i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i) && CanSwitchStance())
                    {
                        SetStance(availableStances[i]);
                    }
                }
            }
        }

        #endregion

        #region Stance Management

        /// <summary>
        /// Set current stance
        /// </summary>
        public void SetStance(CombatStance newStance, bool skipCooldown = false)
        {
            if (newStance == null) return;
            if (newStance == currentStance) return;
            if (!skipCooldown && switchCooldownTimer > 0) return;

            CombatStance previousStance = currentStance;
            currentStance = newStance;

            // Update stance index
            currentStanceIndex = availableStances.IndexOf(newStance);
            if (currentStanceIndex < 0) currentStanceIndex = 0;

            // Apply stance
            ApplyStanceModifiers();

            // Start cooldown
            if (!skipCooldown)
            {
                switchCooldownTimer = switchCooldown;
            }

            // VFX and audio
            PlayStanceSwitchEffects();

            // Trigger event
            OnStanceChanged?.Invoke(currentStance);

            if (debugMode)
            {
                Debug.Log($"[StanceManager] Switched to: {currentStance.StanceName}");
            }
        }

        /// <summary>
        /// Cycle to next available stance
        /// </summary>
        public void CycleToNextStance()
        {
            if (availableStances.Count == 0) return;

            currentStanceIndex = (currentStanceIndex + 1) % availableStances.Count;
            SetStance(availableStances[currentStanceIndex]);
        }

        /// <summary>
        /// Cycle to previous stance
        /// </summary>
        public void CycleToPreviousStance()
        {
            if (availableStances.Count == 0) return;

            currentStanceIndex--;
            if (currentStanceIndex < 0)
                currentStanceIndex = availableStances.Count - 1;

            SetStance(availableStances[currentStanceIndex]);
        }

        /// <summary>
        /// Check if can switch stance now
        /// </summary>
        public bool CanSwitchStance()
        {
            if (switchCooldownTimer > 0) return false;
            if (!canSwitchDuringCombat && playerCombat != null && playerCombat.IsAttacking()) return false;

            return true;
        }

        #endregion

        #region Apply Modifiers

        private void ApplyStanceModifiers()
        {
            if (currentStance == null || baseStats == null) return;

            // Create modified stats from base stats
            modifiedStats = currentStance.ApplyModifiers(baseStats);

            // Apply to player components
            // Note: This is a simplified version. In a real implementation,
            // you'd need to update the actual stats being used by PlayerController
            // and PlayerCombat. This might require refactoring those scripts
            // to use dynamic stats instead of ScriptableObject stats.

            if (debugMode)
            {
                Debug.Log($"[StanceManager] Applied modifiers: " +
                          $"Speed={currentStance.MoveSpeedMultiplier:F2}x, " +
                          $"Damage={currentStance.DamageMultiplier:F2}x");
            }
        }

        #endregion

        #region VFX and Audio

        private void PlayStanceSwitchEffects()
        {
            // Remove old aura
            if (currentAuraVFX != null)
            {
                Destroy(currentAuraVFX);
            }

            // Spawn stance switch flash VFX
            SpawnSwitchVFX();

            // Spawn stance aura if available
            if (currentStance.StanceAuraVFX != null && auraVFXSpawnPoint != null)
            {
                currentAuraVFX = Instantiate(
                    currentStance.StanceAuraVFX,
                    auraVFXSpawnPoint.position,
                    Quaternion.identity,
                    auraVFXSpawnPoint
                );
            }

            // Play sound
            if (currentStance.StanceActivateSound != null)
            {
                AudioSource.PlayClipAtPoint(currentStance.StanceActivateSound, transform.position);
            }
        }

        private void SpawnSwitchVFX()
        {
            // Create a simple color flash effect
            // In a full implementation, you'd have a proper VFX prefab for this
            GameObject flashVFX = new GameObject("StanceSwitchFlash");
            flashVFX.transform.position = transform.position;

            SpriteRenderer sr = flashVFX.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = currentStance.StanceColor;
            sr.sortingOrder = 100;

            // Animate and destroy
            StartCoroutine(AnimateFlashVFX(flashVFX));
        }

        private System.Collections.IEnumerator AnimateFlashVFX(GameObject vfx)
        {
            float elapsed = 0f;
            SpriteRenderer sr = vfx.GetComponent<SpriteRenderer>();
            Vector3 startScale = Vector3.one * 0.5f;
            Vector3 endScale = Vector3.one * 2f;

            while (elapsed < stanceSwitchVFXDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / stanceSwitchVFXDuration;

                vfx.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                Color col = sr.color;
                col.a = 1f - t;
                sr.color = col;

                yield return null;
            }

            Destroy(vfx);
        }

        private Sprite CreateCircleSprite()
        {
            // Create a simple circle texture
            int size = 64;
            Texture2D tex = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pos = new Vector2(x - size / 2, y - size / 2);
                    float dist = pos.magnitude / (size / 2);
                    pixels[y * size + x] = dist < 1f ? Color.white : Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f);
        }

        #endregion

        #region Utility

        private void UpdateCooldown()
        {
            if (switchCooldownTimer > 0)
            {
                switchCooldownTimer -= Time.deltaTime;
            }
        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Get current active stance
        /// </summary>
        public CombatStance GetCurrentStance()
        {
            return currentStance;
        }

        /// <summary>
        /// Get modified stats based on current stance
        /// </summary>
        public GameStats GetModifiedStats()
        {
            return modifiedStats != null ? modifiedStats : baseStats;
        }

        /// <summary>
        /// Add a new stance to available stances
        /// </summary>
        public void UnlockStance(CombatStance stance)
        {
            if (!availableStances.Contains(stance))
            {
                availableStances.Add(stance);

                if (debugMode)
                {
                    Debug.Log($"[StanceManager] Unlocked stance: {stance.StanceName}");
                }
            }
        }

        /// <summary>
        /// Check if stance is unlocked
        /// </summary>
        public bool IsStanceUnlocked(CombatStance stance)
        {
            return availableStances.Contains(stance);
        }

        /// <summary>
        /// Get all available stances
        /// </summary>
        public List<CombatStance> GetAvailableStances()
        {
            return new List<CombatStance>(availableStances);
        }

        #endregion
    }
}
