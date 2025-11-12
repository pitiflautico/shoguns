using UnityEngine;

namespace ShogunsLegacy.Interfaces
{
    /// <summary>
    /// Interface for any entity that can take damage
    /// Used by player, enemies, destructible objects, etc.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage to this entity
        /// </summary>
        /// <param name="damage">Amount of damage to apply</param>
        /// <param name="damageSource">The GameObject that caused the damage</param>
        void TakeDamage(float damage, GameObject damageSource = null);

        /// <summary>
        /// Check if this entity is currently alive
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Get current health value
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// Get maximum health value
        /// </summary>
        float MaxHealth { get; }
    }
}
