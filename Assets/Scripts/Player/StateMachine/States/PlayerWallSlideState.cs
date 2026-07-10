using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Triggered when the player falls while pressing against a wall.
    /// Clamps downward velocity and allows transitioning into a wall jump.
    /// </summary>
    public class PlayerWallSlideState : PlayerState
    {
        public PlayerWallSlideState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Wall Jump: Triggered by pressing jump while sliding
            if (player.InputReader.JumpDown || player.InputReader.HasJumpBuffered)
            {
                player.InputReader.ConsumeJumpBuffer();
                stateMachine.ChangeState(player.WallJumpState);
                return;
            }

            // Landed check
            if (player.IsGrounded)
            {
                stateMachine.ChangeState(player.IdleState);
                return;
            }

            // Fall off wall checks:
            // 1. If we are no longer touching a wall
            // 2. If we stop holding the movement direction towards the wall
            float xInput = player.InputReader.Horizontal;
            bool isPushingWall = (player.WallDirection == 1 && xInput > 0.01f) ||
                                 (player.WallDirection == -1 && xInput < -0.01f);

            if (!player.IsOnWall || !isPushingWall)
            {
                stateMachine.ChangeState(player.FallState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Clamp vertical speed to slide speed limit
            if (player.Rb.velocity.y < -player.Settings.wallSlideSpeed)
            {
                player.SetVelocityY(-player.Settings.wallSlideSpeed);
            }
        }
    }
}
