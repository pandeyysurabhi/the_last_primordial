using Godot;

namespace TheLastPrimordial.Camera;

/// <summary>
/// Camera zone trigger — an Area2D placed in levels that defines
/// camera boundaries for a room/area.
/// 
/// When the player enters this zone, it updates the GameCamera's
/// room limits to match this zone's rect.
/// 
/// Usage: Create an Area2D with a CollisionShape2D (RectangleShape2D)
/// covering the room. Attach this script. Set the room boundaries
/// or let it auto-calculate from the collision shape.
/// </summary>
public partial class CameraZone : Area2D
{
    [ExportGroup("Room Limits")]
    [Export] public bool AutoFromShape { get; set; } = true;
    [Export] public int ManualLeft { get; set; }
    [Export] public int ManualTop { get; set; }
    [Export] public int ManualRight { get; set; } = 320;
    [Export] public int ManualBottom { get; set; } = 180;

    [ExportGroup("Transition")]
    [Export] public bool SmoothTransition { get; set; } = true;
    [Export] public float TransitionDuration { get; set; } = 0.5f;

    private GameCamera? _camera;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not Player.Player) return;

        // Find the game camera
        _camera ??= GetTree().GetFirstNodeInGroup("game_camera") as GameCamera;

        if (_camera == null)
        {
            // Try to find by type
            _camera = GetTree().Root.FindChild("GameCamera", true, false) as GameCamera;
        }

        if (_camera == null)
        {
            GD.PushWarning("CameraZone: Could not find GameCamera!");
            return;
        }

        // Calculate room boundaries
        int left, top, right, bottom;

        if (AutoFromShape)
        {
            var shape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
            if (shape?.Shape is RectangleShape2D rect)
            {
                var halfSize = rect.Size / 2f;
                var pos = shape.GlobalPosition;
                left = (int)(pos.X - halfSize.X);
                top = (int)(pos.Y - halfSize.Y);
                right = (int)(pos.X + halfSize.X);
                bottom = (int)(pos.Y + halfSize.Y);
            }
            else
            {
                // Fallback to manual
                left = ManualLeft;
                top = ManualTop;
                right = ManualRight;
                bottom = ManualBottom;
            }
        }
        else
        {
            left = ManualLeft;
            top = ManualTop;
            right = ManualRight;
            bottom = ManualBottom;
        }

        // Apply limits
        if (SmoothTransition)
        {
            _camera.TransitionToRoomLimits(left, top, right, bottom, TransitionDuration);
        }
        else
        {
            _camera.SetRoomLimits(left, top, right, bottom);
        }
    }
}
