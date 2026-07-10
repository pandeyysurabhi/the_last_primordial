using Godot;
using System.Collections.Generic;

namespace TheLastPrimordial.Core;

/// <summary>
/// Generic state machine that manages state transitions.
/// Add State-derived nodes as children. Set InitialState in the editor.
/// Routes _Process, _PhysicsProcess, and _UnhandledInput to the active state.
/// </summary>
public partial class StateMachine : Node
{
    /// <summary>
    /// Path to the initial state node. Assign in the Godot editor inspector.
    /// </summary>
    [Export]
    public NodePath? InitialStatePath { get; set; }

    /// <summary>
    /// The currently active state.
    /// </summary>
    public State? CurrentState { get; private set; }

    /// <summary>
    /// Previous state for transition context (e.g., knowing if we came from wall slide).
    /// </summary>
    public State? PreviousState { get; private set; }

    private readonly Dictionary<string, State> _states = new();

    public override void _Ready()
    {
        // Register all child states
        foreach (var child in GetChildren())
        {
            if (child is State state)
            {
                state.Machine = this;
                _states[state.Name] = state;
            }
        }

        // Resolve initial state from NodePath
        State? initialState = null;
        if (InitialStatePath != null && !InitialStatePath.IsEmpty)
        {
            initialState = GetNodeOrNull<State>(InitialStatePath);
        }

        // Enter the initial state
        if (initialState != null)
        {
            CurrentState = initialState;
            CurrentState.Enter();
        }
        else if (_states.Count > 0)
        {
            // Fallback: use first child state
            CurrentState = GetChild<State>(0);
            CurrentState.Enter();
        }
    }

    public override void _Process(double delta)
    {
        CurrentState?.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsUpdate(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        CurrentState?.HandleInput(@event);
    }

    /// <summary>
    /// Transition to a new state by node name.
    /// Calls Exit() on current state and Enter() on new state.
    /// </summary>
    public void TransitionTo(string stateName)
    {
        if (!_states.TryGetValue(stateName, out var newState))
        {
            GD.PushError($"StateMachine '{Name}': State '{stateName}' not found.");
            return;
        }

        if (newState == CurrentState) return;

        PreviousState = CurrentState;
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    /// <summary>
    /// Transition to a state by direct reference (avoids string lookup).
    /// </summary>
    public void TransitionTo(State newState)
    {
        if (newState == CurrentState) return;

        PreviousState = CurrentState;
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
