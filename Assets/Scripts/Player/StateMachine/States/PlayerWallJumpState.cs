using UnityEngine;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Handles the trajectory and physics of a wall jump.
    /// Launches the player diagonally away from the wall and temporarily locks steering input.
    /// </summary>
    public class PlayerWallJumpState : PlayerAirState
    {
        private int _wallJumpDirection;
        private bool _hasCutVelocity;

        public PlayerWallJumpState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _hasCutVelocity = false;

            // Jump direction is opposite to the wall we were touching
            // WallDirection is 1 (Right Wall) or -1 (Left Wall). Jump direction should be opposite.
            // If WallDirection is somehow 0, default to opposite of facing direction.
            _wallJumpDirection = player.WallDirection != 0 ? -player.WallDirection : -player.FacingDirection;

            // Lock horizontal steering controls briefly to let the physics-driven trajectory execute
            player.LockMovementControl(player.Settings.wallJumpControlLockDuration);

            // Apply wall jump force diagonally
            float velX = player.Settings.wallJumpForce.x * _wallJumpDirection;
            float velY = player.Settings.wallJumpForce.y;
            player.SetVelocity(velX, velY);

            // Flip the player visuals to face the direction of the wall jump
            player.CheckIfShouldFlip(_wallJumpDirection);
        }

        public override void LogicUpdate()
        {
            // We run base.LogicUpdate to check landing, wall slide transitions, etc.
            // But we override standard air control during the input lock duration.
            base.LogicUpdate();

            // Variable Jump Height: If the player releases jump early, cut upward velocity
            if (player.InputReader.JumpUp && player.Rb.velocity.y > 0.01f && !_hasCutVelocity)
            {
                player.SetVelocityY(player.Rb.velocity.y * player.Settings.jumpCutoffMultiplier);
                _hasCutVelocity = true;
            }

            // Once the upward velocity halts, transition back to normal falling state
            if (player.Rb.velocity.y <= 0f)
            {
                stateMachine.ChangeState(player.FallState);
            }
        }
    }
}
