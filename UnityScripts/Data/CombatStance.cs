using UnityEngine;

namespace ShogunsLegacy.Data
{
    /// <summary>
    /// ScriptableObject defining a combat stance (Water, Earth, Wind, etc.)
    /// Each stance modifies player stats and combat behavior
    /// </summary>
    [CreateAssetMenu(fileName = "New Stance", menuName = "Shoguns Legacy/Combat/Stance")]
    public class CombatStance : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string stanceName = "Default Stance";
        [SerializeField] private string description = "Balanced combat stance";
        [SerializeField] private Sprite stanceIcon;
        [SerializeField] private Color stanceColor = Color.white;

        [Header("Stat Modifiers (Multipliers)")]
        [Tooltip("Movement speed multiplier (1.0 = normal, 1.2 = 20% faster)")]
        [SerializeField] private float moveSpeedMultiplier = 1.0f;

        [Tooltip("Attack speed multiplier")]
        [SerializeField] private float attackSpeedMultiplier = 1.0f;

        [Tooltip("Damage multiplier")]
        [SerializeField] private float damageMultiplier = 1.0f;

        [Tooltip("Defense multiplier (reduces incoming damage)")]
        [SerializeField] private float defenseMultiplier = 1.0f;

        [Tooltip("Critical chance modifier (additive, e.g., 0.1 = +10%)")]
        [SerializeField] private float criticalChanceBonus = 0f;

        [Header("Special Properties")]
        [Tooltip("Super armor during attacks (prevents knockback)")]
        [SerializeField] private bool hasSuperArmor = false;

        [Tooltip("Can parry projectiles")]
        [SerializeField] private bool canParryProjectiles = true;

        [Tooltip("Dodge chance bonus")]
        [SerializeField] private float dodgeChanceBonus = 0f;

        [Header("Attack Modifications")]
        [Tooltip("Attack range multiplier")]
        [SerializeField] private float attackRangeMultiplier = 1.0f;

        [Tooltip("Knockback force multiplier")]
        [SerializeField] private float knockbackMultiplier = 1.0f;

        [Tooltip("Number of attacks in combo chain")]
        [SerializeField] private int comboLength = 3;

        [Header("Stamina (Future Feature)")]
        [Tooltip("Stamina cost per attack")]
        [SerializeField] private float attackStaminaCost = 10f;

        [Tooltip("Stamina regeneration rate")]
        [SerializeField] private float staminaRegenRate = 20f;

        [Header("VFX")]
        [Tooltip("Trail color for attacks in this stance")]
        [SerializeField] private Color attackTrailColor = Color.white;

        [Tooltip("Particle effect when stance is active")]
        [SerializeField] private GameObject stanceAuraVFX;

        [Header("Audio")]
        [Tooltip("Sound when switching to this stance")]
        [SerializeField] private AudioClip stanceActivateSound;

        #region Properties

        public string StanceName => stanceName;
        public string Description => description;
        public Sprite StanceIcon => stanceIcon;
        public Color StanceColor => stanceColor;

        public float MoveSpeedMultiplier => moveSpeedMultiplier;
        public float AttackSpeedMultiplier => attackSpeedMultiplier;
        public float DamageMultiplier => damageMultiplier;
        public float DefenseMultiplier => defenseMultiplier;
        public float CriticalChanceBonus => criticalChanceBonus;

        public bool HasSuperArmor => hasSuperArmor;
        public bool CanParryProjectiles => canParryProjectiles;
        public float DodgeChanceBonus => dodgeChanceBonus;

        public float AttackRangeMultiplier => attackRangeMultiplier;
        public float KnockbackMultiplier => knockbackMultiplier;
        public int ComboLength => comboLength;

        public float AttackStaminaCost => attackStaminaCost;
        public float StaminaRegenRate => staminaRegenRate;

        public Color AttackTrailColor => attackTrailColor;
        public GameObject StanceAuraVFX => stanceAuraVFX;
        public AudioClip StanceActivateSound => stanceActivateSound;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Apply stance modifiers to base stats
        /// </summary>
        public GameStats ApplyModifiers(GameStats baseStats)
        {
            GameStats modifiedStats = Instantiate(baseStats);

            modifiedStats.moveSpeed *= moveSpeedMultiplier;
            modifiedStats.baseDamage *= damageMultiplier;
            modifiedStats.criticalChance += criticalChanceBonus;
            modifiedStats.dodgeChance += dodgeChanceBonus;
            modifiedStats.damageReduction = 1f - ((1f - modifiedStats.damageReduction) * defenseMultiplier);

            return modifiedStats;
        }

        /// <summary>
        /// Get description with stat modifiers formatted
        /// </summary>
        public string GetFormattedDescription()
        {
            string formatted = description + "\n\n";

            if (moveSpeedMultiplier != 1f)
                formatted += $"• Move Speed: {(moveSpeedMultiplier - 1f) * 100f:+0;-0}%\n";

            if (damageMultiplier != 1f)
                formatted += $"• Damage: {(damageMultiplier - 1f) * 100f:+0;-0}%\n";

            if (attackSpeedMultiplier != 1f)
                formatted += $"• Attack Speed: {(attackSpeedMultiplier - 1f) * 100f:+0;-0}%\n";

            if (defenseMultiplier != 1f)
                formatted += $"• Defense: {(1f - defenseMultiplier) * 100f:+0;-0}%\n";

            if (criticalChanceBonus != 0f)
                formatted += $"• Critical Chance: {criticalChanceBonus * 100f:+0;-0}%\n";

            if (hasSuperArmor)
                formatted += "• Super Armor during attacks\n";

            if (dodgeChanceBonus > 0f)
                formatted += $"• Dodge Chance: +{dodgeChanceBonus * 100f:0}%\n";

            return formatted;
        }

        #endregion
    }
}
