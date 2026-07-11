using Godot;

namespace TheLastPrimordial
{
    /// <summary>
    /// An Area2D script that detects the player, displays a floating interaction prompt,
    /// and triggers dialogue or custom logic when the player presses the interact button (W/Up).
    /// </summary>
    public partial class Interactable : Area2D
    {
        [Signal] public delegate void InteractedEventHandler();

        [Export] public string PromptText = "Examine";
        [Export] public string DialogueText = "";
        [Export] public float TextDuration = 3.5f;

        private Label _promptLabel = null!;
        private bool _playerInRange = false;
        private bool _wasEnterPressed = false;

        public override void _Ready()
        {
            // Auto-configure collision layer and mask
            // Player is on Layer 1, Area2D mask should look at Layer 1 (Ground/Player)
            CollisionLayer = 0;
            CollisionMask = 1;

            // Connect signals
            BodyEntered += OnBodyEntered;
            BodyExited += OnBodyExited;

            // Create floating prompt dynamically
            _promptLabel = new Label();
            _promptLabel.Text = $"[ENTER] {PromptText}";
            _promptLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _promptLabel.VerticalAlignment = VerticalAlignment.Center;
            _promptLabel.AddThemeFontSizeOverride("font_size", 11);
            _promptLabel.Position = new Vector2(-60, -38);
            _promptLabel.Size = new Vector2(120, 20);
            _promptLabel.ZIndex = 15;
            _promptLabel.ZAsRelative = false;
            _promptLabel.Visible = false;
            
            // Set text shadow/style
            var style = new StyleBoxFlat();
            style.BgColor = new Color(0.05f, 0.05f, 0.08f, 0.85f);
            style.CornerRadiusTopLeft = 4;
            style.CornerRadiusTopRight = 4;
            style.CornerRadiusBottomLeft = 4;
            style.CornerRadiusBottomRight = 4;
            style.ContentMarginLeft = 6;
            style.ContentMarginRight = 6;
            style.ContentMarginTop = 2;
            style.ContentMarginBottom = 2;
            _promptLabel.AddThemeStyleboxOverride("normal", style);

            AddChild(_promptLabel);
        }

        public override void _Process(double delta)
        {
            if (_playerInRange)
            {
                bool enterPressed = Input.IsKeyPressed(Key.Enter) || Input.IsKeyPressed(Key.KpEnter);
                bool enterJustPressed = enterPressed && !_wasEnterPressed;
                _wasEnterPressed = enterPressed;

                if (Input.IsActionJustPressed("interact") || enterJustPressed)
                {
                    TriggerInteraction();
                }
            }
            else
            {
                _wasEnterPressed = false;
            }
        }

        private void TriggerInteraction()
        {
            GD.Print("[Interactable] Player interacted with: ", Name);
            
            if (!string.IsNullOrEmpty(DialogueText))
            {
                DialogueSystem.Show(DialogueText, TextDuration);
            }

            EmitSignal(SignalName.Interacted);
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body is Player.PlayerController || body.Name.ToString().Contains("Player"))
            {
                _playerInRange = true;
                _promptLabel.Visible = true;
            }
        }

        private void OnBodyExited(Node2D body)
        {
            if (body is Player.PlayerController || body.Name.ToString().Contains("Player"))
            {
                _playerInRange = false;
                _promptLabel.Visible = false;
            }
        }
    }
}
