using Godot;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Base grounded state — shares horizontal input reading, ground-to-air checks,
    /// jump buffering, and attack transition. Identical logic to Unity version.
    /// </summary>
    public abstract class PlayerGroundedState : PlayerState
    {
        protected float HorizontalInput;

        public PlayerGroundedState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            HorizontalInput = Player.InputReader.Horizontal;

            // Attack takes priority
            if (Player.InputReader.AttackDown)
            {
                Player.AttackState.SetComboIndex(0);
                StateMachine.ChangeState(Player.AttackState);
                return;
            }

            // Jump buffering
            if (Player.InputReader.HasJumpBuffered)
            {
                Player.InputReader.ConsumeJumpBuffer();
                StateMachine.ChangeState(Player.JumpState);
                return;
            }

            // Walked off a ledge
            if (!Player.IsGrounded)
            {
                Player.FallState.AllowCoyoteTime = true;
                StateMachine.ChangeState(Player.FallState);
            }
        }
    }
}
