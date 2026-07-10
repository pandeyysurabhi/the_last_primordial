using Godot;

namespace Player.StateMachine.States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            Player.SetVelocityX(0f);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Player.CanMove && Mathf.Abs(HorizontalInput) > 0.01f)
                StateMachine.ChangeState(Player.RunState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            float newVel = Mathf.MoveToward(Player.Velocity.X, 0f, Player.Settings.RunDeceleration * Player.PhysicsDelta);
            Player.SetVelocityX(newVel);
        }
    }
}
