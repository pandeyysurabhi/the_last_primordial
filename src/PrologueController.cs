using Godot;

namespace TheLastPrimordial
{
    /// <summary>
    /// Controls the prologue nightmare level flow, listening for the altar interaction
    /// to transition to Ashen Hollow.
    /// </summary>
    public partial class PrologueController : Node2D
    {
        [Export] public string NextScenePath = "res://scenes/ashen_hollow.tscn";

        private bool _altarFocusTriggered = false;
        private ColorRect _flashOverlay = null!;

        public override void _Ready()
        {
            GameState.Reset();

            // Create DialogueSystem if it isn't set up as an autoload
            if (DialogueSystem.Instance == null)
            {
                var ds = new DialogueSystem();
                AddChild(ds);
            }

            var altarInteractable = GetNodeOrNull<Interactable>("Altar/Interactable");
            if (altarInteractable != null)
            {
                altarInteractable.Interacted += OnAltarInteracted;
            }

            _flashOverlay = GetNode<ColorRect>("CanvasLayer/FlashOverlay");
            _flashOverlay.Visible = false;

            DialogueSystem.Show("Nightmare: The Shattered Sanctuary. Reach the Obsidian Altar.", 4f);
        }

        public override void _Process(double delta)
        {
            if (!_altarFocusTriggered)
            {
                var player = GetNodeOrNull<Player.PlayerController>("Player");
                if (player != null && player.GlobalPosition.X >= 880f && player.IsGrounded)
                {
                    TriggerAltarFocus(player);
                }
            }
        }

        private void TriggerAltarFocus(Player.PlayerController player)
        {
            _altarFocusTriggered = true;
            player.LockMovementControl(1.5f);
            player.SetVelocity(0, 0);

            var camera = player.GetNodeOrNull<Camera2D>("Camera2D");
            if (camera != null)
            {
                var cameraTween = CreateTween();
                cameraTween.TweenProperty(camera, "offset:x", 80f, 0.8f);
            }

            DialogueSystem.Show("The Obsidian Altar looms. The hilt pulses with a desperate call. Press [W] to touch it.", 4.5f);
        }

        private async void OnAltarInteracted()
        {
            DialogueSystem.Show("Muffled Voices: 'Forgive us... Christ... run!'", 3f);
            
            var player = GetNodeOrNull<Player.PlayerController>("Player");
            if (player != null)
            {
                player.LockMovementControl(4.0f);
                player.SetVelocity(0, 0);

                // Screen shake on interaction
                var camera = player.GetNodeOrNull<Camera2D>("Camera2D");
                if (camera != null)
                {
                    var cameraShake = CreateTween();
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 offset = new Vector2((float)GD.RandRange(-8.0, 8.0), (float)GD.RandRange(-8.0, 8.0));
                        cameraShake.TweenProperty(camera, "offset", offset, 0.05f);
                    }
                    cameraShake.TweenProperty(camera, "offset", Vector2.Zero, 0.05f);
                }
            }

            // Screen flash white-violet and fade to black
            _flashOverlay.Visible = true;
            _flashOverlay.Color = new Color(0.95f, 0.9f, 1.0f, 1.0f);
            _flashOverlay.Modulate = new Color(1f, 1f, 1f, 0f);

            var flashTween = CreateTween();
            flashTween.TweenProperty(_flashOverlay, "modulate:a", 1.0f, 0.2f);
            
            await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);

            // Turn flash to solid black
            _flashOverlay.Color = new Color(0, 0, 0, 1);
            
            await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

            // Change scene to Ashen Hollow
            GetTree().ChangeSceneToFile(NextScenePath);
        }
    }
}
