using Godot;

namespace Player.StateMachine.States
{
    public class PlayerWallSlideState : PlayerState
    {
        public PlayerWallSlideState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Wall jump
            if (Player.InputReader.JumpDown || Player.InputReader.HasJumpBuffered)
            {
                Player.InputReader.ConsumeJumpBuffer();
                StateMachine.ChangeState(Player.WallJumpState);
                return;
            }

            // Landed on ground
            if (Player.IsGrounded)
            {
                StateMachine.ChangeState(Player.IdleState);
                return;
            }

            // Fell off wall
            float xInput = Player.InputReader.Horizontal;
            bool isPushingWall = (Player.WallDirection ==  1 && xInput >  0.01f)
                              || (Player.WallDirection == -1 && xInput < -0.01f);

            if (!Player.TouchingWall || !isPushingWall)
                StateMachine.ChangeState(Player.FallState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Clamp downward speed (in Godot, positive Y = down)
            if (Player.Velocity.Y > Player.Settings.WallSlideSpeed)
                Player.SetVelocityY(Player.Settings.WallSlideSpeed);
        }
    }
}
