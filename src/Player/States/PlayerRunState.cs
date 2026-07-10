using Godot;

namespace Player.StateMachine.States
{
    public class PlayerRunState : PlayerGroundedState
    {
        public PlayerRunState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Player.CanMove)
            {
                Player.CheckIfShouldFlip(HorizontalInput);

                if (Mathf.Abs(HorizontalInput) <= 0.01f)
                    StateMachine.ChangeState(Player.IdleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            float physDelta = Player.PhysicsDelta;

            if (Player.CanMove)
            {
                float targetSpeed  = HorizontalInput * Player.Settings.MaxRunSpeed;
                float currentSpeed = Player.Velocity.X;
                bool  isReversing  = (HorizontalInput > 0 && currentSpeed < 0)
                                  || (HorizontalInput < 0 && currentSpeed > 0);
                float rate = isReversing ? Player.Settings.RunDeceleration : Player.Settings.RunAcceleration;

                Player.SetVelocityX(Mathf.MoveToward(currentSpeed, targetSpeed, rate * physDelta));
            }
            else
            {
                Player.SetVelocityX(Mathf.MoveToward(Player.Velocity.X, 0f, Player.Settings.RunDeceleration * physDelta));
            }
        }
    }
}
