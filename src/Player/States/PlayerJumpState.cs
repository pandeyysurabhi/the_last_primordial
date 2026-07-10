using Godot;

namespace Player.StateMachine.States
{
    public class PlayerJumpState : PlayerAirState
    {
        private bool _hasCutVelocity;

        public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            _hasCutVelocity = false;
            Player.SetVelocityY(-Player.Settings.JumpForce); // Godot Y axis is DOWN (+), so negate
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

            // Rising phase ends — fall
            if (Player.Velocity.Y >= 0f)
                StateMachine.ChangeState(Player.FallState);
        }
    }
}
