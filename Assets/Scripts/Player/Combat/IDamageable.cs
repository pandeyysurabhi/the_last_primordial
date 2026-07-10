using UnityEngine;

namespace Player.Combat
{
    /// <summary>
    /// Implement this interface on any entity that can receive damage from Kael's attacks.
    /// Enemies, destructible objects, and bosses should all implement this.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Called when this object is struck.
        /// </summary>
        /// <param name="damage">Raw damage amount before resistances.</param>
        /// <param name="sourcePosition">World position of the attacker. Used for knockback direction.</param>
        void TakeDamage(float damage, Vector3 sourcePosition);

        /// <summary>
        /// Whether this object can currently receive damage (not invincible, not dead, etc.)
        /// </summary>
        bool IsVulnerable { get; }
    }
}
