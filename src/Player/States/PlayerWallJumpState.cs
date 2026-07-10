using Godot;

namespace Player.StateMachine.States
{
    public class PlayerWallJumpState : PlayerAirState
    {
        private int  _wallJumpDirection;
        private bool _hasCutVelocity;

        public PlayerWallJumpState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            _hasCutVelocity = false;

            _wallJumpDirection = Player.WallDirection != 0 ? -Player.WallDirection : -Player.FacingDirection;

            Player.LockMovementControl(Player.Settings.WallJumpControlLockDuration);

            // Godot Y: negative = up
            float velX = Player.Settings.WallJumpForce.X * _wallJumpDirection;
            float velY = -Player.Settings.WallJumpForce.Y;
            Player.SetVelocity(velX, velY);

            Player.CheckIfShouldFlip(_wallJumpDirection);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Variable jump height
            if (Player.InputReader.JumpUp && Player.Velocity.Y < -0.01f && !_hasCutVelocity)
            {
                Player.SetVelocityY(Player.Velocity.Y * Player.Settings.JumpCutoffMultiplier);
                _hasCutVelocity = true;
            }

            if (Player.Velocity.Y >= 0f)
                StateMachine.ChangeState(Player.FallState);
        }
    }
}
