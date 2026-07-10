using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Death state — player has died. Plays death animation, disables input.
/// Emits a signal for the game manager to handle (respawn, game over screen).
/// </summary>
public partial class PlayerDeathState : PlayerState
{
    private float _deathTimer;
    private const float DeathAnimDuration = 1.0f;

    [Signal] public delegate void DeathCompleteEventHandler();

    public override void Enter()
    {
        _deathTimer = DeathAnimDuration;

        // Stop all movement
        P.Velocity = Vector2.Zero;

        // Disable combat systems
        P.ComboTracker.ResetCombo();

        // Visual: fade to red tint then fade out
        P.Sprite.Modulate = new Color(0.8f, 0.2f, 0.2f, 1f);

        GD.Print("Kael has fallen...");
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);
        P.MoveAndSlide();

        _deathTimer -= (float)delta;

        // Fade out
        float alpha = Mathf.Max(0f, _deathTimer / DeathAnimDuration);
        P.Sprite.Modulate = new Color(0.8f, 0.2f, 0.2f, alpha);

        if (_deathTimer <= 0f)
        {
            EmitSignal(SignalName.DeathComplete);
        }
    }

    // No HandleInput — dead players can't act
}
