using Godot;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Fall state — player is descending (not on ground).
/// Supports coyote time jump and jump buffering.
/// Transitions to: Land, WallSlide, Jump (coyote/buffer), Dash, Attack.
/// </summary>
public partial class PlayerFallState : PlayerState
{
    public override void Enter()
    {
        // Play fall animation
    }

    public override void PhysicsUpdate(double delta)
    {
        // Apply stronger gravity when falling (snappier feel, like Hollow Knight)
        P.ApplyGravity(delta, P.FastFallMultiplier);

        // Air control
        float dir = P.GetInputDirection();
        P.UpdateFacing(dir);
        P.ApplyHorizontalMovement(delta, dir, P.MoveSpeed);

        // Transition: landed
        if (P.IsOnFloor())
        {
            // Check jump buffer — if player pressed jump just before landing
            if (P.HasJumpBuffer)
            {
                Machine?.TransitionTo("Jump");
            }
            else
            {
                Machine?.TransitionTo("Land");
            }
            return;
        }

        // Transition: touching wall → WallSlide
        if (P.IsTouchingWall() && dir != 0f)
        {
            Machine?.TransitionTo("WallSlide");
            return;
        }

        P.MoveAndSlide();
    }

    public override void HandleInput(InputEvent @event)
    {
        // Coyote time jump — can still jump briefly after leaving a ledge
        if (@event.IsActionPressed("jump"))
        {
            if (P.HasCoyoteTime)
            {
                Machine?.TransitionTo("Jump");
                return;
            }
            else
            {
                // Buffer the jump input for when we land
                P.JumpBufferTimer = P.JumpBufferTime;
            }
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
    }
}
