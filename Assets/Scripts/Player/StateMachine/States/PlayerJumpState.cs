using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Handles the upward phase of a jump.
    /// Implements variable jump height by cutting vertical velocity when the jump button is released early.
    /// </summary>
    public class PlayerJumpState : PlayerAirState
    {
        private bool _hasCutVelocity;

        public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _hasCutVelocity = false;
            
            // Apply initial upward jump force
            player.SetVelocityY(player.Settings.jumpForce);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Variable Jump Height: If player releases the jump button while still rising, cut velocity
            if (player.InputReader.JumpUp && player.Rb.velocity.y > 0.01f && !_hasCutVelocity)
            {
                player.SetVelocityY(player.Rb.velocity.y * player.Settings.jumpCutoffMultiplier);
                _hasCutVelocity = true;
            }

            // Transition to falling state once upward momentum halts
            if (player.Rb.velocity.y <= 0f)
            {
                stateMachine.ChangeState(player.FallState);
            }
        }
    }
}
