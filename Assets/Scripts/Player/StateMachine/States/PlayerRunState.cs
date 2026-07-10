using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Grounded state when horizontal input is active.
    /// Accelerates the player up to the maximum run speed and handles orientation flipping.
    /// </summary>
    public class PlayerRunState : PlayerGroundedState
    {
        public PlayerRunState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (player.CanMove)
            {
                // Flip the character's visual scale to face movement input direction
                player.CheckIfShouldFlip(horizontalInput);

                // Transition back to Idle if movement input is released
                if (Mathf.Abs(horizontalInput) <= 0.01f)
                {
                    stateMachine.ChangeState(player.IdleState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (player.CanMove)
            {
                float targetSpeed = horizontalInput * player.Settings.maxRunSpeed;
                float currentSpeed = player.Rb.velocity.x;

                // Determine whether to use acceleration or deceleration coefficient
                // Even while running, if input direction changes, we use deceleration to slow down before reversing
                bool isReversing = (horizontalInput > 0 && currentSpeed < 0) || (horizontalInput < 0 && currentSpeed > 0);
                float speedRate = isReversing ? player.Settings.runDeceleration : player.Settings.runAcceleration;

                float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedRate * Time.fixedDeltaTime);
                player.SetVelocityX(newSpeed);
            }
            else
            {
                // Decelerate if control is locked (e.g. wall jump transition phase)
                float newSpeed = Mathf.MoveTowards(player.Rb.velocity.x, 0f, player.Settings.runDeceleration * Time.fixedDeltaTime);
                player.SetVelocityX(newSpeed);
            }
        }
    }
}
