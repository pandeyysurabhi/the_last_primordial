using Godot;
using Player;

namespace TheLastPrimordial
{
    /// <summary>
    /// Manages the final escape level, triggering falling stones that require
    /// Chronostasis Pulse to freeze, and ending the chapter when the border is crossed.
    /// </summary>
    public partial class ForestEscapeController : Node2D
    {
        private Area2D _finishTrigger = null!;
        private ColorRect _fadeOverlay = null!;

        private Node2D _sevenEyes = null!;
        private Area2D _eyesTrigger = null!;
        private ColorRect _lightningOverlay = null!;
        private ColorRect _fatherSilhouette = null!;
        private bool _eyesRevealed = false;

        public override void _Ready()
        {
            if (DialogueSystem.Instance == null)
            {
                var ds = new DialogueSystem();
                AddChild(ds);
            }

            _finishTrigger = GetNode<Area2D>("FinishTrigger");
            _finishTrigger.BodyEntered += OnFinishTriggerEntered;

            _fadeOverlay = GetNode<ColorRect>("CanvasLayer/FadeOverlay");
            _fadeOverlay.Visible = false;

            _sevenEyes = GetNode<Node2D>("Background/SevenEyes");
            _eyesTrigger = GetNode<Area2D>("HazardTriggers/EyesTrigger");
            _lightningOverlay = GetNode<ColorRect>("CanvasLayer/LightningOverlay");
            _lightningOverlay.Visible = false;
            _fatherSilhouette = GetNode<ColorRect>("Environment/FatherSilhouette");
            _fatherSilhouette.Visible = false;

            _eyesTrigger.BodyEntered += OnEyesTriggerEntered;

            // Wire up triggers
            var trigger1 = GetNodeOrNull<Area2D>("HazardTriggers/Trigger1");
            if (trigger1 != null)
            {
                trigger1.BodyEntered += (body) => {
                    if (body is Player.PlayerController) DropHazard(1);
                };
            }

            var trigger2 = GetNodeOrNull<Area2D>("HazardTriggers/Trigger2");
            if (trigger2 != null)
            {
                trigger2.BodyEntered += (body) => {
                    if (body is Player.PlayerController) DropHazard(2);
                };
            }

            DialogueSystem.Show("Sentinels are descending! Escape to the East! Freeze falling debris with [E] to crawl under!", 5f);
        }

        private void DropHazard(int index)
        {
            var hazard = GetNodeOrNull<TimeFreezableHazard>($"Hazards/HazardBlock{index}");
            if (hazard != null)
            {
                GD.Print("[ForestEscapeController] Dropping hazard: ", hazard.Name);
                hazard.ApplyGravity = true;
                hazard.Gravity = 400f; // Slower drop to give player reaction time
            }
        }

        private void OnEyesTriggerEntered(Node2D body)
        {
            if (body is Player.PlayerController && !_eyesRevealed)
            {
                _eyesRevealed = true;
                DialogueSystem.Show("A rift tears open in the heavens... Seven burning eyes look down upon you.", 4.5f);

                // Reveal eyes in sky
                _sevenEyes.Visible = true;
                _sevenEyes.Modulate = new Color(1f, 1f, 1f, 0f);
                var tween = CreateTween();
                tween.TweenProperty(_sevenEyes, "modulate:a", 1.0f, 2.0f);

                // Screen shake by shaking camera
                var player = body as Player.PlayerController;
                var camera = player?.GetNodeOrNull<Camera2D>("Camera2D");
                if (camera != null)
                {
                    var cameraShake = CreateTween();
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 offset = new Vector2((float)GD.RandRange(-6.0, 6.0), (float)GD.RandRange(-6.0, 6.0));
                        cameraShake.TweenProperty(camera, "offset", offset, 0.05f);
                    }
                    cameraShake.TweenProperty(camera, "offset", Vector2.Zero, 0.05f);
                }
            }
        }

        private async void OnFinishTriggerEntered(Node2D body)
        {
            if (body is Player.PlayerController)
            {
                GD.Print("[ForestEscapeController] Escape completed!");

                // Disable player movement
                var player = GetNodeOrNull<Player.PlayerController>("Player");
                if (player != null)
                {
                    player.LockMovementControl(9.0f);
                    player.SetVelocity(0, 0);
                }

                // White screen flash overlay
                _lightningOverlay.Visible = true;
                _lightningOverlay.Modulate = new Color(1f, 1f, 1f, 1f);
                var flash = CreateTween();
                flash.TweenProperty(_lightningOverlay, "modulate:a", 0.0f, 1.0f);

                // Wait 1 second
                await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

                // Spawn spectral father silhouette
                _fatherSilhouette.Visible = true;
                _fatherSilhouette.Modulate = new Color(1f, 1f, 1f, 0f);
                var silhouetteTween = CreateTween();
                silhouetteTween.TweenProperty(_fatherSilhouette, "modulate:a", 0.7f, 1.0f);

                DialogueSystem.Show("Alaric: 'Hatred makes decisions easy, Christ. Truth makes them difficult. Keep the hilt... keep the path.'", 5.0f);

                // Wait 5 seconds
                await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

                // Fade out father and fade in black escape overlay
                var meltTween = CreateTween();
                meltTween.TweenProperty(_fatherSilhouette, "modulate:a", 0.0f, 1.0f);

                _fadeOverlay.Visible = true;
                _fadeOverlay.Modulate = new Color(0f, 0f, 0f, 0f);
                var blackTween = CreateTween();
                blackTween.TweenProperty(_fadeOverlay, "modulate:a", 1.0f, 2.0f);

                await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);

                DialogueSystem.Show("THE LAST PRIMORDIAL - Chapter 1 Completed.", 6f);
            }
        }
    }
}
