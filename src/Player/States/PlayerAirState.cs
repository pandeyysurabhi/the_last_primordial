using Godot;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Base airborne state — handles air horizontal control, gravity scaling,
    /// landing checks, and wall-slide transitions.
    /// Godot port: Rigidbody2D.velocity → CharacterBody2D.Velocity,
    ///             gravityScale → PlayerController.GravityScale.
    /// </summary>
    public abstract class PlayerAirState : PlayerState
    {
        protected float HorizontalInput;

        public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void Exit()
        {
            Player.GravityScale = 1f;
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            HorizontalInput = Player.InputReader.Horizontal;

            if (Player.CanMove)
                Player.CheckIfShouldFlip(HorizontalInput);

            // Landed
            if (Player.IsGrounded && Player.Velocity.Y <= 0.01f)
            {
                StateMachine.ChangeState(Player.IdleState);
                return;
            }

            // Wall slide
            if (Player.TouchingWall && Player.Velocity.Y < 0.01f)
            {
                bool isPushingWall = (Player.WallDirection ==  1 && HorizontalInput >  0.01f)
                                  || (Player.WallDirection == -1 && HorizontalInput < -0.01f);
                if (isPushingWall)
                {
                    StateMachine.ChangeState(Player.WallSlideState);
                    return;
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Gravity scaling: heavy fall, lighter rise
            Player.GravityScale = Player.Velocity.Y > 0f
                ? Player.Settings.FallGravityScale
                : Player.Settings.JumpGravityScale;

            // Air horizontal control
            if (Player.CanMove)
            {
                float targetSpeed  = HorizontalInput * Player.Settings.MaxRunSpeed;
                float currentSpeed = Player.Velocity.X;
                float rate = Mathf.Abs(HorizontalInput) > 0.01f
                    ? Player.Settings.AirAcceleration
                    : Player.Settings.AirDeceleration;

                Player.SetVelocityX(Mathf.MoveToward(currentSpeed, targetSpeed, rate * Player.PhysicsDelta));
            }
        }
    }
}
