using Godot;

namespace TheLastPrimordial.Core;

/// <summary>
/// Abstract base class for all states in the state machine.
/// Each state is a Node child of a StateMachine node.
/// Override virtual methods to define state behavior.
/// </summary>
public abstract partial class State : Node
{
    /// <summary>
    /// Reference to the parent state machine. Set automatically by StateMachine.
    /// </summary>
    public StateMachine? Machine { get; set; }

    /// <summary>
    /// Called when this state becomes the active state.
    /// Use for initialization, playing animations, resetting timers.
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// Called when this state is being replaced by another state.
    /// Use for cleanup.
    /// </summary>
    public virtual void Exit() { }

    /// <summary>
    /// Called every frame (via _Process). Use for non-physics logic,
    /// animation updates, timer checks, input polling.
    /// </summary>
    public virtual void Update(double delta) { }

    /// <summary>
    /// Called every physics frame (via _PhysicsProcess). Use for
    /// movement, velocity changes, MoveAndSlide calls.
    /// </summary>
    public virtual void PhysicsUpdate(double delta) { }

    /// <summary>
    /// Called for unhandled input events. Use for action-triggered
    /// transitions (jump pressed, attack pressed, etc).
    /// </summary>
    public virtual void HandleInput(InputEvent @event) { }
}
