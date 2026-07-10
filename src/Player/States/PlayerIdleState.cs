using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Idle state — player is standing still on the ground.
/// Transitions to: Run, Jump, Fall, Attack, Dash, Block.
/// </summary>
public partial class PlayerIdleState : PlayerState
{
    public override void Enter()
    {
        // Play idle animation (placeholder: just stop movement)
        P.Velocity = new Vector2(0, P.Velocity.Y);
    }

    public override void PhysicsUpdate(double delta)
    {
        P.ApplyGravity(delta);

        float dir = P.GetInputDirection();

        // Transition: movement input → Run
        if (Mathf.Abs(dir) > 0.1f)
        {
            Machine?.TransitionTo("Run");
            return;
        }

        // Transition: not on floor → Fall
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
        // Jump
        if (@event.IsActionPressed("jump") && P.IsOnFloor())
        {
            Machine?.TransitionTo("Jump");
            return;
        }

        // Dash
        if (@event.IsActionPressed("dodge") && P.CanDash)
        {
            Machine?.TransitionTo("Dash");
            return;
        }

        // Light attack
        if (@event.IsActionPressed("attack_light"))
        {
            Machine?.TransitionTo("Attack");
            return;
        }

        // Heavy attack
        if (@event.IsActionPressed("attack_heavy"))
        {
            Machine?.TransitionTo("Attack");
            return;
        }

        // Block
        if (@event.IsActionPressed("block"))
        {
            Machine?.TransitionTo("Block");
            return;
        }
    }
}
