namespace Player.StateMachine.States
{
    public class PlayerFallState : PlayerAirState
    {
        public bool AllowCoyoteTime { get; set; }

        public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public override void Exit()
        {
            AllowCoyoteTime = false;
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (AllowCoyoteTime)
            {
                if (Elapsed <= Player.Settings.CoyoteTimeDuration)
                {
                    if (Player.InputReader.JumpDown || Player.InputReader.HasJumpBuffered)
                    {
                        AllowCoyoteTime = false;
                        Player.InputReader.ConsumeJumpBuffer();
                        StateMachine.ChangeState(Player.JumpState);
                        return;
                    }
                }
                else
                {
                    AllowCoyoteTime = false;
                }
            }
        }
    }
}
