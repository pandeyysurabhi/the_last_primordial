using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Base airborne state. Handles air horizontal control, gravity scaling based on y-velocity,
    /// landing checks, and wall-sliding transitions.
    /// </summary>
    public abstract class PlayerAirState : PlayerState
    {
        protected float horizontalInput;

        public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            // Reset gravity scale to default when leaving air state
            player.Rb.gravityScale = 1f;
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            horizontalInput = player.InputReader.Horizontal;

            if (player.CanMove)
            {
                player.CheckIfShouldFlip(horizontalInput);
            }

            // Check if player landed on ground
            if (player.IsGrounded && player.Rb.velocity.y <= 0.01f)
            {
                // Landed. Transition to Idle state (which automatically moves to Run if input exists)
                stateMachine.ChangeState(player.IdleState);
                return;
            }

            // Check if player should slide against a wall
            // Occurs when pushing horizontal input towards a wall while falling
            if (player.IsOnWall && player.Rb.velocity.y < 0.01f)
            {
                bool isPushingWall = (player.WallDirection == 1 && horizontalInput > 0.01f) ||
                                     (player.WallDirection == -1 && horizontalInput < -0.01f);

                if (isPushingWall)
                {
                    stateMachine.ChangeState(player.WallSlideState);
                    return;
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Apply responsive gravity scaling
            // Falling uses heavier gravity than rising to create a sharp, satisfying weight feel
            if (player.Rb.velocity.y < 0f)
            {
                player.Rb.gravityScale = player.Settings.fallGravityScale;
            }
            else
            {
                player.Rb.gravityScale = player.Settings.jumpGravityScale;
            }

            // Apply horizontal air control
            if (player.CanMove)
            {
                float targetSpeed = horizontalInput * player.Settings.maxRunSpeed;
                float currentSpeed = player.Rb.velocity.x;

                // Select air coefficients
                float speedRate = Mathf.Abs(horizontalInput) > 0.01f ? player.Settings.airAcceleration : player.Settings.airDeceleration;

                float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedRate * Time.fixedDeltaTime);
                player.SetVelocityX(newSpeed);
            }
        }
    }
}
