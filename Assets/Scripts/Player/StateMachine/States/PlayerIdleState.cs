using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Grounded state when the player has no horizontal movement input.
    /// Snappily decelerates the player to a standstill.
    /// </summary>
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.SetVelocityX(0f); // Optional: instant stop or let it slide depending on deceleration
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Transition to Run state if horizontal input is applied
            if (player.CanMove && Mathf.Abs(horizontalInput) > 0.01f)
            {
                stateMachine.ChangeState(player.RunState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Smoothly decelerate to zero
            float currentVel = player.Rb.velocity.x;
            float newVel = Mathf.MoveTowards(currentVel, 0f, player.Settings.runDeceleration * Time.fixedDeltaTime);
            player.SetVelocityX(newVel);
        }
    }
}
