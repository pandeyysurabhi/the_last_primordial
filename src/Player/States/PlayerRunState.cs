using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Run state — player is moving horizontally on the ground.
/// Uses acceleration/friction for responsive but not instant movement.
/// Transitions to: Idle, Jump, Fall, Attack, Dash, Block.
/// </summary>
public partial class PlayerRunState : PlayerState
{
    public override void Enter()
    {
        // Play run animation
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);

        float dir = P.GetInputDirection();

        // Update facing direction
        P.UpdateFacing(dir);

        // Apply movement
        P.ApplyHorizontalMovement(delta, dir, P.MoveSpeed);

        // Transition: no input → Idle
        if (Mathf.Abs(dir) < 0.1f)
        {
            Machine?.TransitionTo("Idle");
            return;
        }

        // Transition: not on floor → Fall (with coyote time)
        if (!P.IsOnFloor())
        {
            P.CoyoteTimer = P.CoyoteTime;
            Machine?.TransitionTo("Fall");
            return;
        }

        P.MoveAndSlide();
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump") && P.IsOnFloor())
        {
            Machine?.TransitionTo("Jump");
            return;
        }

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

        if (@event.IsActionPressed("block"))
        {
            Machine?.TransitionTo("Block");
            return;
        }
    }
}
