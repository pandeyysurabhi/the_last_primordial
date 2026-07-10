using UnityEngine;
using Player.Combat;


namespace Player.StateMachine.States
{
    /// <summary>
    /// Handles Kael's melee sword attack.
    ///
    /// Flow:
    ///   Enter  → freeze/slow horizontal movement, trigger "isAttacking" animator bool
    ///   Update → activate hitbox at the correct time window, check combo input,
    ///            transition back to Idle/Run when duration elapses
    ///   Exit   → deactivate hitbox, clear animator bool
    ///
    /// Combo support: if the player presses attack again inside the combo window
    /// after the hitbox phase ends, comboQueued is set. On Exit the combo is
    /// chained by immediately re-entering AttackState with an incremented combo index.
    /// </summary>
    public class PlayerAttackState : PlayerState
    {
        private KaelCombatSettings _combat;
        private bool _hitboxActive;
        private bool _comboQueued;
        private int  _comboIndex;

        // Cached results for efficiency
        private readonly Collider2D[] _hitResults = new Collider2D[16];

        public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _combat      = player.CombatSettings;
            _hitboxActive = false;
            _comboQueued  = false;

            // Pass combo index to animator so blends can select different attack clips
            if (player.Animator != null)
                player.Animator.SetInteger("comboIndex", _comboIndex);

            // Lock movement while attacking on the ground
            if (player.IsGrounded && _combat != null && _combat.lockMovementOnGroundAttack)
                player.SetVelocityX(0f);
        }

        public override void Exit()
        {
            base.Exit();

            // Deactivate hitbox if still on
            if (_hitboxActive)
            {
                _hitboxActive = false;
            }

            if (player.Animator != null)
                player.Animator.SetInteger("comboIndex", 0);

            // Chain into combo if queued
            if (_comboQueued && _combat != null && _comboIndex < _combat.maxComboCount - 1)
            {
                // Re-enter attack with next combo index
                player.AttackState.SetComboIndex(_comboIndex + 1);
                stateMachine.ChangeState(player.AttackState);
            }
        }

        public override void LogicUpdate()
        {
            float elapsed = UnityEngine.Time.time - startTime;

            if (_combat == null)
            {
                // Fallback if no settings: return to idle after 0.45s
                if (elapsed >= 0.45f) stateMachine.ChangeState(player.IdleState);
                return;
            }

            // ── Hitbox window ────────────────────────────────────────────────
            bool shouldBeActive = elapsed >= _combat.hitboxActiveStart
                                  && elapsed < _combat.hitboxActiveEnd;

            if (shouldBeActive && !_hitboxActive)
            {
                _hitboxActive = true;
                PerformHitboxCheck();
            }
            else if (!shouldBeActive && _hitboxActive)
            {
                _hitboxActive = false;
            }

            // ── Combo window ─────────────────────────────────────────────────
            // Queue a follow-up attack if the player presses attack after the
            // hitbox phase but before the full attack duration ends
            if (elapsed >= _combat.hitboxActiveEnd && elapsed < _combat.attackDuration)
            {
                if (player.InputReader.AttackDown && _comboIndex < _combat.maxComboCount - 1)
                    _comboQueued = true;
            }

            // ── End of attack ────────────────────────────────────────────────
            if (elapsed >= _combat.attackDuration)
            {
                // Go back to the appropriate locomotion state
                if (player.IsGrounded)
                    stateMachine.ChangeState(
                        Mathf.Abs(player.InputReader.Horizontal) > 0.01f
                            ? (PlayerState)player.RunState
                            : player.IdleState);
                else
                    stateMachine.ChangeState(player.FallState);
            }
        }

        public override void PhysicsUpdate()
        {
            if (_combat == null) return;

            // Slow air movement during attack
            if (!player.IsGrounded)
            {
                float targetX = player.Rb.velocity.x * _combat.airAttackSpeedMultiplier;
                player.SetVelocityX(
                    Mathf.MoveTowards(player.Rb.velocity.x, targetX,
                        player.Settings.airDeceleration * UnityEngine.Time.fixedDeltaTime));
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        /// <summary>Sets which combo hit this is before entering the state.</summary>
        public void SetComboIndex(int index) => _comboIndex = index;

        private void PerformHitboxCheck()
        {
            if (_combat == null) return;

            // Offset hitbox in facing direction
            Vector2 hitboxOffset = new Vector2(
                _combat.hitboxOffset.x * player.FacingDirection,
                _combat.hitboxOffset.y);

            Vector2 hitboxCentre = (Vector2)player.transform.position + hitboxOffset;

            int count = Physics2D.OverlapBoxNonAlloc(
                hitboxCentre, _combat.hitboxSize, 0f,
                _hitResults, _combat.hittableLayers);

            for (int i = 0; i < count; i++)
            {
                Collider2D col = _hitResults[i];
                if (col == null || col.gameObject == player.gameObject) continue;

                // Apply knockback through Rigidbody2D if present
                Rigidbody2D rb = col.attachedRigidbody;
                if (rb != null)
                {
                    Vector2 knockDir = new Vector2(player.FacingDirection, 0.3f).normalized;
                    rb.AddForce(knockDir * _combat.knockbackForce, ForceMode2D.Impulse);
                }

                // Notify any IDamageable component on the hit object
                IDamageable damageable = col.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(_combat.attackDamage, player.transform.position);

                UnityEngine.Debug.Log($"[Kael Attack] Hit: {col.gameObject.name}  Damage: {_combat.attackDamage}");
            }
        }
    }
}
