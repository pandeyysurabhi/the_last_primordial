using Godot;

namespace Player
{
    /// <summary>
    /// Represents a gravity anchor point that Kael can grapple onto.
    /// Registers itself in the "GravityRift" group automatically.
    /// </summary>
    public partial class GravityRift : Node2D
    {
        public override void _Ready()
        {
            AddToGroup("GravityRift");
            GD.Print("[GravityRift] Anchor initialized at: ", GlobalPosition);
        }
    }
}
