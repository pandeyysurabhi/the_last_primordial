using Godot;
using TheLastPrimordial.Core;

namespace TheLastPrimordial.Player.States;

/// <summary>
/// Base class for all player states. Provides a typed reference to the Player node.
/// </summary>
public abstract partial class PlayerState : State
{
    protected Player P { get; private set; } = null!;

    public override void _Ready()
    {
        // Walk up the tree: State → StateMachine → Player
        P = (Player)GetParent().GetParent();
    }
}
