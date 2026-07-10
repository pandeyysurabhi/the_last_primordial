using Godot;

namespace Player.StateMachine.States
{
    /// <summary>
    /// Handles the attack state including hitbox timing, combo window, and knockback.
    /// Godot port: Physics2D.OverlapBoxNonAlloc → PhysicsDirectSpaceState2D query,
    ///             Rigidbody2D.AddForce → CharacterBody2D velocity impulse.
    /// </summary>
    public class PlayerAttackState : PlayerState
    {
        private KaelCombatSettings? _combat;
        private int  _comboIndex;
        private bool _hitboxActive;
        private bool _comboQueued;

        public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }

        public void SetComboIndex(int index) => _comboIndex = index;

        public override void Enter()
        {
            base.Enter();

            _combat       = Player.CombatSettings;
            _hitboxActive = false;
            _comboQueued  = false;

            if (_combat != null && _combat.LockMovementOnGroundAttack && Player.IsGrounded)
                Player.SetVelocityX(0f);
        }

        public override void Exit()
        {
            base.Exit();
            _hitboxActive = false;

            // Chain combo
            if (_comboQueued && _combat != null && _comboIndex < _combat.MaxComboCount - 1)
            {
                Player.AttackState.SetComboIndex(_comboIndex + 1);
                StateMachine.ChangeState(Player.AttackState);
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (_combat == null)
            {
                if (Elapsed >= 0.45) StateMachine.ChangeState(Player.IdleState);
                return;
            }

            // Hitbox window
            bool shouldBeActive = Elapsed >= _combat.HitboxActiveStart
                                && Elapsed <  _combat.HitboxActiveEnd;

            if (shouldBeActive && !_hitboxActive)
            {
                _hitboxActive = true;
                PerformHitboxCheck();
            }
            else if (!shouldBeActive && _hitboxActive)
            {
                _hitboxActive = false;
            }

            // Combo queue window
            if (Elapsed >= _combat.HitboxActiveEnd && Elapsed < _combat.AttackDuration)
            {
                if (Player.InputReader.AttackDown && _comboIndex < _combat.MaxComboCount - 1)
                    _comboQueued = true;
            }

            // End of attack
            if (Elapsed >= _combat.AttackDuration)
            {
                if (Player.IsGrounded)
                    StateMachine.ChangeState(Mathf.Abs(Player.InputReader.Horizontal) > 0.01f
                        ? (PlayerState)Player.RunState
                        : Player.IdleState);
                else
                    StateMachine.ChangeState(Player.FallState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_combat == null) return;

            // Slow horizontal air movement during attack
            if (!Player.IsGrounded)
            {
                float target    = Player.Velocity.X * _combat.AirAttackSpeedMultiplier;
                Player.SetVelocityX(Mathf.MoveToward(Player.Velocity.X, target,
                    Player.Settings.AirDeceleration * Player.PhysicsDelta));
            }
        }

        // ── Hitbox ────────────────────────────────────────────────────────────
        private void PerformHitboxCheck()
        {
            if (_combat == null) return;

            // Build the query using Godot's physics space
            var spaceState = Player.GetWorld2D().DirectSpaceState;

            Vector2 hitboxCentre = Player.GlobalPosition + new Vector2(
                _combat.HitboxOffset.X * Player.FacingDirection,
                _combat.HitboxOffset.Y);

            var shape    = new RectangleShape2D();
            shape.Size   = _combat.HitboxSize;

            var query    = new PhysicsShapeQueryParameters2D();
            query.Shape  = shape;
            query.Transform = new Transform2D(0f, hitboxCentre);
            query.CollisionMask = _combat.HittableLayers;
            query.Exclude.Add(Player.GetRid()); // Don't hit ourselves

            var results  = spaceState.IntersectShape(query);

            foreach (var result in results)
            {
                var collider = result["collider"].AsGodotObject() as Node;
                if (collider == null) continue;

                GD.Print($"[Kael Attack] Hit: {collider.Name}  Damage: {_combat.AttackDamage}");

                // Apply knockback via velocity if it's a CharacterBody2D
                if (collider is CharacterBody2D body)
                {
                    Vector2 knockDir = new Vector2(Player.FacingDirection, -0.3f).Normalized();
                    body.Velocity += knockDir * _combat.KnockbackForce;
                }
            }
        }
    }
}
