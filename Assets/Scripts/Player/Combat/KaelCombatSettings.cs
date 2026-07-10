using UnityEngine;

namespace Player
{
    /// <summary>
    /// Designer-tunable combat parameters for Kael.
    /// Create via: Assets → Create → Player → Kael Combat Settings
    /// </summary>
    [CreateAssetMenu(fileName = "KaelCombatSettings", menuName = "Player/Kael Combat Settings")]
    public class KaelCombatSettings : ScriptableObject
    {
        [Header("Attack Timing")]
        [Tooltip("Total seconds the attack state lasts before returning to Idle/Run.")]
        public float attackDuration = 0.45f;

        [Tooltip("Time offset from attack start when the hitbox becomes active.")]
        public float hitboxActiveStart = 0.1f;

        [Tooltip("Time offset from attack start when the hitbox becomes inactive.")]
        public float hitboxActiveEnd = 0.35f;

        [Header("Attack Physics")]
        [Tooltip("If true, Kael briefly stops horizontal movement during an attack (grounded only).")]
        public bool lockMovementOnGroundAttack = true;

        [Tooltip("Horizontal velocity multiplier applied while attacking in the air (0 = full stop, 1 = full speed).")]
        [Range(0f, 1f)]
        public float airAttackSpeedMultiplier = 0.3f;

        [Header("Hitbox")]
        [Tooltip("Size of the melee hitbox (world-space).")]
        public Vector2 hitboxSize = new Vector2(1.6f, 0.9f);

        [Tooltip("Offset from the player pivot to the hitbox centre (in facing direction).")]
        public Vector2 hitboxOffset = new Vector2(0.9f, 0f);

        [Tooltip("Layers that can be hit by this attack.")]
        public LayerMask hittableLayers;

        [Header("Damage")]
        [Tooltip("Base damage dealt by a standard ground attack.")]
        public float attackDamage = 15f;

        [Tooltip("Force magnitude of the knockback applied to hit targets.")]
        public float knockbackForce = 8f;

        [Header("Combo")]
        [Tooltip("Time window after an attack ends where another attack input will chain into a combo.")]
        public float comboWindowDuration = 0.2f;

        [Tooltip("Maximum number of hits in a single ground combo chain.")]
        public int maxComboCount = 3;
    }
}
