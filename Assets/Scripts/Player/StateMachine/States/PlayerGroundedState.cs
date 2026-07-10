using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Base grounded state sharing horizontal input handling, ground-to-air checks,
    /// and jump-trigger checks.
    /// </summary>
    public abstract class PlayerGroundedState : PlayerState
    {
        protected float horizontalInput;

        public PlayerGroundedState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            // Re-zero the vertical velocity when touching ground to prevent gravity accumulation
            // but keep minor downward force if needed to snap to slopes (can adjust based on project needs)
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            horizontalInput = player.InputReader.Horizontal;

            // Attack takes priority over jump/movement
            if (player.InputReader.AttackDown)
            {
                player.AttackState.SetComboIndex(0);
                stateMachine.ChangeState(player.AttackState);
                return;
            }

            // Jump buffering evaluation: if jump is buffered, jump immediately
            if (player.InputReader.HasJumpBuffered)
            {
                player.InputReader.ConsumeJumpBuffer();
                stateMachine.ChangeState(player.JumpState);
                return;
            }

            // Ground checking: if we leave the ground without jumping, transition to fall state
            if (!player.IsGrounded)
            {
                // Enable coyote time because we walked off a ledge, we didn't jump
                player.FallState.AllowCoyoteTime = true;
                stateMachine.ChangeState(player.FallState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
