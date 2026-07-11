using Godot;

namespace TheLastPrimordial
{
    /// <summary>
    /// Attached to StaticBody2D nodes to handle phase shift (Dimensional Slip) triggers,
    /// synchronizing visual opacity and collision state.
    /// </summary>
    public partial class PhasePlatform : StaticBody2D
    {
        [Export] public bool IsSpectralPlatform = false;

        private CollisionShape2D? _collisionShape;
        private Control? _visualRect;
        private CpuParticles2D? _particles;

        public override void _Ready()
        {
            AddToGroup("PhaseActive");

            // Cache child node references
            _collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
            _visualRect = GetNodeOrNull<Control>("Visual");

            // Setup dynamic floating particles to represent dimensional slip
            if (IsSpectralPlatform && _visualRect != null)
            {
                _particles = new CpuParticles2D();
                _particles.Amount = 8;
                _particles.Lifetime = 1.2f;
                _particles.EmissionShape = CpuParticles2D.EmissionShapeEnum.Rectangle;
                _particles.EmissionRectExtents = new Vector2(_visualRect.Size.X / 2f, 4f);
                _particles.Direction = new Vector2(0, -1);
                _particles.Spread = 15f;
                _particles.Gravity = new Vector2(0, 0);
                _particles.InitialVelocityMin = 5f;
                _particles.InitialVelocityMax = 15f;
                _particles.ScaleAmountMin = 0.5f;
                _particles.ScaleAmountMax = 1.5f;
                _particles.Color = new Color(0.7f, 0.4f, 1.0f, 0.4f);
                AddChild(_particles);
                
                // Position it along the platform surface
                _particles.Position = new Vector2(0, -_visualRect.Size.Y / 2f);
            }

            // Sync initial state (starting phase is physical)
            UpdatePhaseState(false, true); // Instant setup
        }

        public void OnPhaseChanged(bool isSpectralActive)
        {
            UpdatePhaseState(isSpectralActive, false);
        }

        private void UpdatePhaseState(bool isSpectralActive, bool instant)
        {
            // Spectral platforms are active only when spectral phase is active
            // Physical platforms are active only when spectral phase is inactive
            bool shouldBeActive = IsSpectralPlatform ? isSpectralActive : !isSpectralActive;

            // Toggle particles
            if (_particles != null)
            {
                _particles.Emitting = shouldBeActive;
            }

            // Toggle physics collision safely using SetDeferred
            if (_collisionShape != null)
            {
                _collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, !shouldBeActive);
            }

            float targetAlpha = shouldBeActive ? 1.0f : 0.2f;
            Color baseColor = IsSpectralPlatform ? new Color(0.7f, 0.3f, 1.0f) : new Color(0.8f, 0.8f, 0.8f);
            Color targetColor = new Color(baseColor.R, baseColor.G, baseColor.B, targetAlpha);

            if (instant)
            {
                Modulate = targetColor;
                if (_visualRect != null)
                {
                    _visualRect.Modulate = targetColor;
                }
            }
            else
            {
                // Smooth fade transition matching active status
                var tween = CreateTween().SetParallel(true);
                tween.TweenProperty(this, "modulate", targetColor, 0.2f);
                if (_visualRect != null)
                {
                    tween.TweenProperty(_visualRect, "modulate", targetColor, 0.2f);
                }
            }
        }
    }
}
