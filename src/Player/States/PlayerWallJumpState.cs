using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Wall Jump state — kick off a wall with directional force.
/// Brief input lockout to prevent immediately re-sticking to the same wall.
/// Transitions to: Fall, WallSlide (re-contact different wall).
/// </summary>
public partial class PlayerWallJumpState : PlayerState
{
    private int _jumpDirection;

    public override void Enter()
    {
        // Jump away from the wall
        int wallDir = P.GetWallDirection();
        _jumpDirection = -wallDir;

        P.Velocity = new Vector2(
            _jumpDirection * P.WallJumpHorizontalForce,
            -P.WallJumpVerticalForce
        );

        // Lock input briefly to prevent immediately re-sticking
        P.WallJumpLockTimer = P.WallJumpInputLockTime;

        // Face jump direction
        P.UpdateFacing(_jumpDirection);

        P.EmitSignal(Player.SignalName.WallJumped);
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);

        // Allow air control only after input lock expires
        if (P.WallJumpLockTimer <= 0f)
        {
            float dir = P.GetInputDirection();
            P.UpdateFacing(dir);
            P.ApplyHorizontalMovement(delta, dir, P.MoveSpeed);
        }

        // Transition: falling
        if (P.Velocity.Y >= 0)
        {
            Machine?.TransitionTo("Fall");
            return;
        }

        // Transition: re-contact a wall (allow re-slide for wall climbing chains)
        if (P.IsTouchingWall() && P.WallJumpLockTimer <= 0f)
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
