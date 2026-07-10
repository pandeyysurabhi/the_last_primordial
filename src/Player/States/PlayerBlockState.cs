using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Block state — Sekiro-style blocking and deflecting.
/// Pressing block opens a perfect deflect window (~0.2s).
/// Holding block reduces incoming damage but costs posture.
/// Perfect deflects deal posture damage TO the attacker.
///
/// Transitions to: Idle (release block), Hurt (posture broken),
/// Attack (counter-attack after deflect).
/// </summary>
public partial class PlayerBlockState : PlayerState
{
    /// <summary>Whether a perfect deflect just occurred (for counter-attack window).</summary>
    private bool _justDeflected;
    private float _counterAttackWindow;
    private const float CounterAttackDuration = 0.4f;

    public override void Enter()
    {
        _justDeflected = false;
        _counterAttackWindow = 0f;

        // Activate the deflect system
        P.DeflectSystem.StartBlock();

        // Slow movement while blocking
        P.Velocity = new Vector2(P.Velocity.X * 0.3f, P.Velocity.Y);

        // Connect deflect signals
        P.DeflectSystem.PerfectDeflect += OnPerfectDeflect;
        P.DeflectSystem.BlockedHit += OnBlockedHit;
    }

    public override void Exit()
    {
        P.DeflectSystem.StopBlock();

        // Disconnect signals
        P.DeflectSystem.PerfectDeflect -= OnPerfectDeflect;
        P.DeflectSystem.BlockedHit -= OnBlockedHit;
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);

        // Very slow movement while blocking
        float dir = P.GetInputDirection();
        P.ApplyHorizontalMovement(delta, dir, P.MoveSpeed * 0.2f);

        // Counter-attack window timer
        if (_justDeflected)
        {
            _counterAttackWindow -= (float)delta;
            if (_counterAttackWindow <= 0f)
            {
                _justDeflected = false;
            }
        }

        // Check posture break
        if (P.HealthSystem.IsPostureBroken)
        {
            Machine?.TransitionTo("Hurt");
            return;
        }

        P.MoveAndSlide();
    }

    public override void HandleInput(InputEvent @event)
    {
        // Release block → return to idle
        if (@event.IsActionReleased("block"))
        {
            if (P.IsOnFloor())
            {
                Machine?.TransitionTo("Idle");
            }
            else
            {
                Machine?.TransitionTo("Fall");
            }
            return;
        }

        // Counter-attack after perfect deflect (Sekiro Mikiri-style)
        if (_justDeflected && (@event.IsActionPressed("attack_light") || @event.IsActionPressed("attack_heavy")))
        {
            Machine?.TransitionTo("Attack");
            return;
        }

        // Can dodge out of block
        if (@event.IsActionPressed("dodge") && P.CanDash)
        {
            Machine?.TransitionTo("Dash");
            return;
        }
    }

    private void OnPerfectDeflect(Combat.HitboxComponent attackerHitbox)
    {
        _justDeflected = true;
        _counterAttackWindow = CounterAttackDuration;

        // Visual feedback: brief white flash
        P.Sprite.Modulate = new Color(2f, 2f, 2f, 1f); // Bright flash

        // Reset flash after brief time
        var tween = CreateTween();
        tween.TweenProperty(P.Sprite, "modulate", Colors.White, 0.1f);

        // Style meter bonus
        P.StyleMeter.AddDeflectPoints(true);

        GD.Print("PERFECT DEFLECT!");
    }

    private void OnBlockedHit(Combat.HitboxComponent attackerHitbox)
    {
        // Apply blocked damage to self
        P.HealthSystem.TakeDamage(
            attackerHitbox.Damage * P.DeflectSystem.BlockDamageMultiplier,
            P.DeflectSystem.BlockSelfPostureDamage,
            Vector2.Zero // No knockback on block
        );

        P.StyleMeter.AddDeflectPoints(false);
    }
}
