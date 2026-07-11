using Godot;

namespace Player
{
    /// <summary>
    /// Script that can be attached to falling masonry, moving hazards, or enemies.
    /// Responds to the Chronostasis Pulse (Time Stop) by pausing logic and physics.
    /// </summary>
    public partial class TimeFreezableHazard : CharacterBody2D
    {
        [Export] public bool ApplyGravity = true;
        [Export] public float Gravity = 800f;
        [Export] public Vector2 ConstantVelocity = Vector2.Zero;

        private Vector2 _savedVelocity;
        private bool _isFrozen = false;

        public override void _Ready()
        {
            AddToGroup("TimeFreezable");
            Velocity = ConstantVelocity;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_isFrozen)
            {
                // Freeze in space, do not process physics
                return;
            }

            if (ApplyGravity)
            {
                if (!IsOnFloor())
                    Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * (float)delta);
            }
            else
            {
                // Horizontal ping-pong when colliding with walls
                if (IsOnWall())
                {
                    ConstantVelocity = new Vector2(-ConstantVelocity.X, ConstantVelocity.Y);
                    Velocity = ConstantVelocity;
                }
                else
                {
                    Velocity = ConstantVelocity;
                }
            }

            MoveAndSlide();
        }

        public void Freeze()
        {
            if (_isFrozen) return;
            _isFrozen = true;
            _savedVelocity = Velocity;
            Velocity = Vector2.Zero;

            // Pause animations
            var animSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
            if (animSprite != null) animSprite.SpeedScale = 0f;

            var animPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
            if (animPlayer != null) animPlayer.SpeedScale = 0f;

            // Visual indicator: shift to desaturated frozen blue
            Modulate = new Color(0.4f, 0.7f, 1.0f, 1.0f);
        }

        public void Unfreeze()
        {
            if (!_isFrozen) return;
            _isFrozen = false;
            Velocity = _savedVelocity;

            // Unpause animations
            var animSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
            if (animSprite != null) animSprite.SpeedScale = 1f;

            var animPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
            if (animPlayer != null) animPlayer.SpeedScale = 1f;

            // Restore color
            Modulate = new Color(1f, 1f, 1f, 1f);
        }
    }
}
