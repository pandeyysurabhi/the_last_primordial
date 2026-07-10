using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Handles the falling phase of airborne movement.
    /// Tracks coyote time window to allow late jumps.
    /// </summary>
    public class PlayerFallState : PlayerAirState
    {
        /// <summary>
        /// Set to true by grounded states if the player walks off a ledge.
        /// Allows coyote time jumping for a brief window.
        /// </summary>
        public bool AllowCoyoteTime { get; set; }

        public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            // Reset coyote time permission
            AllowCoyoteTime = false;
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Coyote Time: Check if the player can perform a late jump
            if (AllowCoyoteTime)
            {
                if (Time.time - startTime <= player.Settings.coyoteTimeDuration)
                {
                    // Catch either immediate press or buffered press in coyote window
                    if (player.InputReader.JumpDown || player.InputReader.HasJumpBuffered)
                    {
                        AllowCoyoteTime = false;
                        player.InputReader.ConsumeJumpBuffer();
                        stateMachine.ChangeState(player.JumpState);
                        return;
                    }
                }
                else
                {
                    // Window expired
                    AllowCoyoteTime = false;
                }
            }
        }
    }
}
