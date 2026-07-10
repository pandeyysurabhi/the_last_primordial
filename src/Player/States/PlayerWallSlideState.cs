using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Wall Slide state — player clings to a wall and slides down slowly.
/// Hollow Knight-style: must push toward wall to cling, slow descent.
/// Transitions to: WallJump, Fall (move away), Idle (landed), Dash.
/// </summary>
public partial class PlayerWallSlideState : PlayerState
{
    private int _wallDirection;

    public override void Enter()
    {
        _wallDirection = P.GetWallDirection();

        // Face away from wall (like Hollow Knight)
        P.UpdateFacing(-_wallDirection);
    }

    public override void PhysicsUpdate(double delta)
    {
        // Cap fall speed to wall slide speed
        float velY = P.Velocity.Y;
        if (velY < 0)
        {
            // Still going up — apply normal gravity
            velY += P.GravityScale * (float)delta;
        }
        else
        {
            // Sliding down — clamp to wall slide speed
            velY = Mathf.Min(velY + P.GravityScale * 0.5f * (float)delta, P.WallSlideSpeed);
        }
        P.Velocity = new Vector2(0, velY);

        float dir = P.GetInputDirection();

        // Transition: landed
        if (P.IsOnFloor())
        {
            Machine?.TransitionTo("Idle");
            return;
        }

        // Transition: lost wall contact or moving away from wall
        if (!P.IsTouchingWall() || (int)dir == -_wallDirection)
        {
            Machine?.TransitionTo("Fall");
            return;
        }

        P.MoveAndSlide();
    }

    public override void HandleInput(InputEvent @event)
    {
        // Wall jump
        if (@event.IsActionPressed("jump"))
        {
            Machine?.TransitionTo("WallJump");
            return;
        }

        // Dash off wall
        if (@event.IsActionPressed("dodge") && P.CanDash)
        {
            // Face away from wall before dashing
            P.FacingDirection = -_wallDirection;
            Machine?.TransitionTo("Dash");
            return;
        }
    }
}
