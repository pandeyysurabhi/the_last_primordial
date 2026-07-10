using Godot;

namespace TheLastPrimordial;

/// <summary>
/// Main entry point for The Last Primordial.
/// This node serves as the root scene controller.
/// </summary>
public partial class Main : Node2D
{
    public override void _Ready()
    {
        GD.Print("═══════════════════════════════════════════");
        GD.Print("  The Last Primordial — Initialized");
        GD.Print("  Engine: Godot 4.3 (.NET/C#)");
        GD.Print("  Resolution: 320×180 (4x scale)");
        GD.Print("═══════════════════════════════════════════");
    }
}
