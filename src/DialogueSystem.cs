using Godot;

namespace TheLastPrimordial
{
	/// <summary>
	/// A simple UI overlay that displays dialogues/narrative subtitles at the bottom of the screen.
	/// Uses dynamic node creation to run out-of-the-box without requiring pre-made scenes.
	/// </summary>
	public partial class DialogueSystem : CanvasLayer
	{
		private static DialogueSystem _instance = null!;
		public static DialogueSystem Instance
		{
			get
			{
				if (_instance == null || !GodotObject.IsInstanceValid(_instance))
				{
					return null!;
				}
				return _instance;
			}
			private set => _instance = value;
		}

		private Label _label = null!;
		private PanelContainer _panel = null!;
		private SceneTreeTimer? _activeTimer;

		public override void _Ready()
		{
			Instance = this;

			// Visual container for dialogue background
			_panel = new PanelContainer();
			_panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomWide, Control.LayoutPresetMode.KeepSize, 20);
			_panel.OffsetBottom = -10;
			_panel.OffsetTop = -50;
			_panel.OffsetLeft = 30;
			_panel.OffsetRight = -30;

			var style = new StyleBoxFlat();
			style.BgColor = new Color(0.06f, 0.04f, 0.12f, 0.9f);
			style.SetBorderWidthAll(1);
			style.BorderColor = new Color(0.6f, 0.1f, 0.9f, 1f); // Purple border matching Choice/Chaos theme
			style.CornerRadiusTopLeft = 3;
			style.CornerRadiusTopRight = 3;
			style.CornerRadiusBottomLeft = 3;
			style.CornerRadiusBottomRight = 3;
			style.ContentMarginLeft = 8;
			style.ContentMarginRight = 8;
			_panel.AddThemeStyleboxOverride("panel", style);

			// Label for subtitle text
			_label = new Label();
			_label.HorizontalAlignment = HorizontalAlignment.Center;
			_label.VerticalAlignment = VerticalAlignment.Center;
			_label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			
			// Set font size smaller for 180p viewport
			_label.AddThemeFontSizeOverride("font_size", 8);
			_label.Text = "";
			_panel.AddChild(_label);

			AddChild(_panel);
			_panel.Visible = false;
		}

		public override void _ExitTree()
		{
			if (_instance == this)
			{
				_instance = null!;
			}
		}

		public static void Show(string text, float duration = 3.5f)
		{
			var inst = Instance;
			if (inst != null && GodotObject.IsInstanceValid(inst))
			{
				inst.ShowText(text, duration);
			}
		}

		public void ShowText(string text, float duration = 3.5f)
		{
			_label.Text = text;
			_panel.Visible = true;

			// Timer to hide dialog automatically
			_activeTimer = GetTree().CreateTimer(duration);
			_activeTimer.Timeout += OnTimerTimeout;
		}

		private void OnTimerTimeout()
		{
			_panel.Visible = false;
			_label.Text = "";
			_activeTimer = null;
		}
	}
}
