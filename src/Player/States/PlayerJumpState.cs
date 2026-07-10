using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Jump state — ascending phase of a jump.
/// Variable-height jump: releasing jump early cuts upward velocity.
/// Transitions to: Fall, WallSlide, Attack (aerial), Dash.
/// </summary>
public partial class PlayerJumpState : PlayerState
{
    private bool _jumpCut;

    public override void Enter()
    {
        _jumpCut = false;

        // Apply jump impulse
        P.Velocity = new Vector2(P.Velocity.X, -P.JumpForce);
        P.CoyoteTimer = 0f;
        P.JumpBufferTimer = 0f;
        P.EmitSignal(Player.SignalName.Jumped);
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);

        // Air control
        float dir = P.GetInputDirection();
        P.UpdateFacing(dir);
        P.ApplyHorizontalMovement(delta, dir, P.MoveSpeed);

        // Variable height jump: cut velocity when jump released
        if (!_jumpCut && Input.IsActionJustReleased("jump") && P.Velocity.Y < 0)
        {
            _jumpCut = true;
            P.Velocity = new Vector2(P.Velocity.X, P.Velocity.Y * P.JumpCutMultiplier);
        }

        // Transition: velocity going down → Fall
        if (P.Velocity.Y >= 0)
        {
            Machine?.TransitionTo("Fall");
            return;
        }

        // Transition: touching wall → WallSlide
        if (P.IsTouchingWall() && !P.IsOnFloor())
        {
            Machine?.TransitionTo("WallSlide");
            return;
        }

        P.MoveAndSlide();
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("dodge") && P.CanDash)
        {
            Machine?.TransitionTo("Dash");
            return;
        }

        if (@event.IsActionPressed("attack_light") || @event.IsActionPressed("attack_heavy"))
        {
            Machine?.TransitionTo("Attack");
            return;
        }
    }
}
