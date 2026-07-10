using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Land state — brief animation when touching ground after a fall.
/// Almost instant transition to Idle or Run.
/// </summary>
public partial class PlayerLandState : PlayerState
{
    private float _landTimer;
    private const float LandDuration = 0.05f; // Very brief squash frame

    public override void Enter()
    {
        _landTimer = LandDuration;
        P.Velocity = new Vector2(P.Velocity.X, 0);
        P.EmitSignal(Player.SignalName.Landed);
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);
        P.MoveAndSlide();

        _landTimer -= (float)delta;
        if (_landTimer <= 0f)
        {
            float dir = P.GetInputDirection();
            if (Mathf.Abs(dir) > 0.1f)
            {
                Machine?.TransitionTo("Run");
            }
            else
            {
                Machine?.TransitionTo("Idle");
            }
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        // Allow instant jump out of land (responsive feel)
        if (@event.IsActionPressed("jump"))
        {
            Machine?.TransitionTo("Jump");
            return;
        }

        if (@event.IsActionPressed("attack_light") || @event.IsActionPressed("attack_heavy"))
        {
            Machine?.TransitionTo("Attack");
            return;
        }
    }
}
