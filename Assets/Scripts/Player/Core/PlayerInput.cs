using UnityEngine;

namespace Player
{
    /// <summary>
    /// Implements IPlayerInput using Unity's legacy Input Manager.
    /// Manages jump buffer internally for frame-perfect input windows.
    ///
    /// Default mappings (configurable in Project Settings → Input Manager):
    ///   Horizontal  → A/D or Left/Right arrows (or gamepad L-stick X)
    ///   Vertical    → W/S or Up/Down arrows     (or gamepad L-stick Y)
    ///   Jump        → Space / gamepad South
    ///   Attack      → Z or J  / gamepad West (X on Xbox, Square on PS)
    /// </summary>
    public class PlayerInput : MonoBehaviour, IPlayerInput
    {
        [Header("Input Action Names (match Project Settings → Input Manager)")]
        [SerializeField] private string horizontalAxis = "Horizontal";
        [SerializeField] private string verticalAxis   = "Vertical";
        [SerializeField] private string jumpButton     = "Jump";
        [SerializeField] private string attackButton   = "Attack";  // Add "Attack" in Input Manager

        [Header("Buffer")]
        [Tooltip("How long a jump input remains valid after being pressed.")]
        [SerializeField] private float jumpBufferDuration = 0.15f;

        // ── IPlayerInput Properties ───────────────────────────────────────────
        public float Horizontal  { get; private set; }
        public float Vertical    { get; private set; }
        public bool  JumpDown    { get; private set; }
        public bool  JumpUp      { get; private set; }
        public bool  JumpHeld    { get; private set; }
        public bool  AttackDown  { get; private set; }

        private float _jumpBufferTimer;
        public bool HasJumpBuffered => _jumpBufferTimer > 0f;

        // ── Unity Lifecycle ───────────────────────────────────────────────────
        private void Update()
        {
            // Axes
            Horizontal = Input.GetAxisRaw(horizontalAxis);
            Vertical   = Input.GetAxisRaw(verticalAxis);

            // Jump — caught in Update so no frame is skipped between physics ticks
            JumpDown = Input.GetButtonDown(jumpButton);
            JumpUp   = Input.GetButtonUp(jumpButton);
            JumpHeld = Input.GetButton(jumpButton);

            // Attack
            // Fallback: if "Attack" axis isn't configured, check Z or J keys directly
            AttackDown = TryGetButtonDown(attackButton)
                         || Input.GetKeyDown(KeyCode.Z)
                         || Input.GetKeyDown(KeyCode.J);

            // Buffer timer
            if (JumpDown)
                _jumpBufferTimer = jumpBufferDuration;
            else if (_jumpBufferTimer > 0f)
                _jumpBufferTimer -= Time.deltaTime;
        }

        public void ConsumeJumpBuffer() => _jumpBufferTimer = 0f;

        // ── Private Helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Safe GetButtonDown that doesn't throw if the button name isn't registered.
        /// </summary>
        private static bool TryGetButtonDown(string buttonName)
        {
            try   { return Input.GetButtonDown(buttonName); }
            catch { return false; }
        }
    }
}
