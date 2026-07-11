using Godot;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Player state for the Gravity Tether (Vector Pull).
    /// Pulls the player rapidly toward a target rift, disabling gravity.
    /// Releasing early launches the player forward with momentum.
    /// </summary>
    public class PlayerTetherState : PlayerState
    {
        private Node2D? _targetRift;
        private float _pullSpeed = 1500f; // Rapid acceleration
        private float _maxSpeed = 600f;   // High velocity cap
        private float _launchBoost = 1.3f; // Multiplier on exit velocity

        public PlayerTetherState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public void SetTargetRift(Node2D rift)
        {
            _targetRift = rift;
        }

        public override void Enter()
        {
            base.Enter();
            Player.GravityScale = 0f; // Suspend gravity during pull
        }

        public override void Exit()
        {
            Player.GravityScale = 1f;
            _targetRift = null;
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Press jump to launch out with extra speed (Tether Jump)
            if (Player.InputReader.JumpDown && _targetRift != null)
            {
                Vector2 dirToRift = (_targetRift.GlobalPosition - Player.GlobalPosition).Normalized();
                
                // Project forward/upward momentum
                float jumpForce = Player.Settings.JumpForce * 1.3f;
                float xSpeed = Player.InputReader.Horizontal * Player.Settings.MaxRunSpeed * 1.4f;
                if (xSpeed == 0f) 
                {
                    xSpeed = dirToRift.X * Player.Settings.MaxRunSpeed * 1.4f;
                }
                
                Player.Velocity = new Vector2(xSpeed, -jumpForce);
                Player.LockMovementControl(0.2f); // Lock controls for satisfying leap momentum
                StateMachine.ChangeState(Player.FallState);
                return;
            }

            // Release tether -> launch!
            if (!Player.InputReader.TetherHeld || _targetRift == null)
            {
                // Apply launch velocity boost
                Player.Velocity *= _launchBoost;
                
                // Lock control briefly for smooth launch momentum
                Player.LockMovementControl(0.15f);

                StateMachine.ChangeState(Player.FallState);
                return;
            }

            // Landed on floor
            if (Player.IsGrounded)
            {
                StateMachine.ChangeState(Player.IdleState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_targetRift == null) return;

            Vector2 toRift = _targetRift.GlobalPosition - Player.GlobalPosition;
            Vector2 dir = toRift.Normalized();

            // Accelerate towards gravity rift anchor
            Player.Velocity = Player.Velocity.MoveToward(dir * _maxSpeed, _pullSpeed * Player.PhysicsDelta);

            // Flip facing direction based on movement
            if (Mathf.Abs(Player.Velocity.X) > 0.01f)
            {
                Player.CheckIfShouldFlip(Player.Velocity.X);
            }
        }
    }
}
