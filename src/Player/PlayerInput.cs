using Godot;

namespace Player
{
    /// <summary>
    /// Reads Godot Input actions and feeds them through IPlayerInput.
    /// Manages jump buffer internally for frame-perfect input windows.
    ///
    /// Input actions (configure in Project → Project Settings → Input Map):
    ///   move_left / move_right  → A/D or Arrow Keys
    ///   jump                    → Space
    ///   attack                  → Z or J
    /// </summary>
    public partial class PlayerInput : Node, IPlayerInput
    {
        [Export] public float JumpBufferDuration = 0.15f;

        // ── IPlayerInput Properties ───────────────────────────────────────────
        public float Horizontal      { get; private set; }
        public float Vertical        { get; private set; }
        public bool  JumpDown        { get; private set; }
        public bool  JumpUp          { get; private set; }
        public bool  JumpHeld        { get; private set; }
        public bool  AttackDown      { get; private set; }

        private float _jumpBufferTimer;
        public bool HasJumpBuffered => _jumpBufferTimer > 0f;

        // ── Godot Lifecycle ───────────────────────────────────────────────────
        public override void _Process(double delta)
        {
            Horizontal = Input.GetAxis("move_left", "move_right");
            Vertical   = 0f; // Unused in a platformer; no move_up/move_down actions needed

            JumpDown = Input.IsActionJustPressed("jump");
            JumpUp   = Input.IsActionJustReleased("jump");
            JumpHeld = Input.IsActionPressed("jump");

            AttackDown = Input.IsActionJustPressed("attack");

            // Buffer timer
            if (JumpDown)
                _jumpBufferTimer = JumpBufferDuration;
            else if (_jumpBufferTimer > 0f)
                _jumpBufferTimer -= (float)delta;
        }

        public void ConsumeJumpBuffer() => _jumpBufferTimer = 0f;
    }
}
