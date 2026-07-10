using UnityEngine;

namespace Player
{
    /// <summary>
    /// Configuration data for character movement.
    /// ScriptableObjects allow runtime tweaking of parameters and multiple presets.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "Player/Movement Settings")]
    public class PlayerMovementSettings : ScriptableObject
    {
        [Header("Horizontal Movement")]
        [Tooltip("Maximum horizontal speed when running on the ground.")]
        public float maxRunSpeed = 9f;

        [Tooltip("Rate of acceleration when running on the ground.")]
        public float runAcceleration = 80f;

        [Tooltip("Rate of deceleration when slowing down on the ground.")]
        public float runDeceleration = 60f;

        [Tooltip("Rate of acceleration when in the air.")]
        public float airAcceleration = 50f;

        [Tooltip("Rate of deceleration when slowing down in the air.")]
        public float airDeceleration = 30f;

        [Header("Jumping")]
        [Tooltip("Initial upward velocity applied when jumping.")]
        public float jumpForce = 15f;

        [Tooltip("Gravity scale when rising during a jump.")]
        public float jumpGravityScale = 2.5f;

        [Tooltip("Gravity scale when falling.")]
        public float fallGravityScale = 3.5f;

        [Range(0f, 1f)]
        [Tooltip("How much upward velocity is retained when the jump button is released early. Lower = sharper cutoff.")]
        public float jumpCutoffMultiplier = 0.5f;

        [Tooltip("Duration (in seconds) that the player can still jump after falling off a ledge.")]
        public float coyoteTimeDuration = 0.12f;

        [Header("Wall Mechanics")]
        [Tooltip("Maximum downward velocity when sliding down a wall.")]
        public float wallSlideSpeed = 3f;

        [Tooltip("Force vector applied during a wall jump. X = force away from wall, Y = force upward.")]
        public Vector2 wallJumpForce = new Vector2(8f, 15f);

        [Tooltip("Duration (in seconds) that player horizontal control is locked after a wall jump to prevent steering back.")]
        public float wallJumpControlLockDuration = 0.15f;

        [Header("Collision Detection")]
        [Tooltip("Offset of the ground check box relative to the player's pivot.")]
        public Vector2 groundCheckOffset = new Vector2(0f, -0.9f);

        [Tooltip("Size of the ground check box.")]
        public Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);

        [Tooltip("Offset of the wall check box relative to the player's pivot. Horizontal offset direction will automatically mirror facing direction.")]
        public Vector2 wallCheckOffset = new Vector2(0.45f, 0f);

        [Tooltip("Size of the wall check box.")]
        public Vector2 wallCheckSize = new Vector2(0.1f, 1.4f);

        [Tooltip("Layer mask representing ground surfaces.")]
        public LayerMask groundLayer;

        [Tooltip("Layer mask representing wall surfaces.")]
        public LayerMask wallLayer;
    }
}
