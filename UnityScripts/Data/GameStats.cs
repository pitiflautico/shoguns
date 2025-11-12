using UnityEngine;

namespace ShogunsLegacy.Data
{
    /// <summary>
    /// ScriptableObject to hold entity stats (player, enemies, etc.)
    /// Can be shared between instances or unique per entity
    /// </summary>
    [CreateAssetMenu(fileName = "New Stats", menuName = "Shoguns Legacy/Stats/Entity Stats")]
    public class GameStats : ScriptableObject
    {
        [Header("Combat Stats")]
        [Tooltip("Maximum health points")]
        public float maxHealth = 100f;

        [Tooltip("Base damage dealt by attacks")]
        public float baseDamage = 10f;

        [Tooltip("Attack speed multiplier (1 = normal, 2 = double speed)")]
        public float attackSpeed = 1f;

        [Tooltip("Critical hit chance (0-1)")]
        [Range(0f, 1f)]
        public float criticalChance = 0.1f;

        [Tooltip("Critical damage multiplier")]
        public float criticalMultiplier = 2f;

        [Header("Movement Stats")]
        [Tooltip("Movement speed in units per second")]
        public float moveSpeed = 5f;

        [Tooltip("Can this entity dash/dodge?")]
        public bool canDash = true;

        [Tooltip("Dash cooldown in seconds")]
        public float dashCooldown = 1f;

        [Tooltip("Dash distance")]
        public float dashDistance = 3f;

        [Header("Defense Stats")]
        [Tooltip("Damage reduction percentage (0-1)")]
        [Range(0f, 1f)]
        public float damageReduction = 0f;

        [Tooltip("Chance to dodge attacks (0-1)")]
        [Range(0f, 1f)]
        public float dodgeChance = 0f;

        /// <summary>
        /// Create a runtime copy of these stats that can be modified
        /// </summary>
        public GameStats CreateRuntimeCopy()
        {
            return Instantiate(this);
        }

        /// <summary>
        /// Calculate actual damage after applying critical chance
        /// </summary>
        public float CalculateDamage()
        {
            float damage = baseDamage;

            // Check for critical hit
            if (Random.value < criticalChance)
            {
                damage *= criticalMultiplier;
            }

            return damage;
        }

        /// <summary>
        /// Apply damage reduction to incoming damage
        /// </summary>
        public float ApplyDamageReduction(float incomingDamage)
        {
            return incomingDamage * (1f - damageReduction);
        }

        /// <summary>
        /// Check if an attack is dodged
        /// </summary>
        public bool RollDodge()
        {
            return Random.value < dodgeChance;
        }
    }
}
