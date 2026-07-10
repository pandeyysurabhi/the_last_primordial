#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Player.Editor
{
    /// <summary>
    /// One-click setup wizard that:
    ///   1. Slices kael_spritesheet.png into individual sprites
    ///   2. Creates AnimationClips for Idle and Attack
    ///   3. Builds a fully-wired AnimatorController with transitions
    ///   4. Creates the Kael Player prefab with all required components
    ///
    /// Usage: Unity menu → Tools → Kael → Setup Kael Player
    /// </summary>
    public static class KaelSetupWizard
    {
        // ── Paths ──────────────────────────────────────────────────────────────
        private const string SpritesheetPath  = "Assets/Sprites/Characters/Kael/kael_spritesheet.png";
        private const string AnimClipsDir     = "Assets/Animations/Characters/Kael";
        private const string AnimControllerPath = "Assets/Animations/Characters/Kael/Kael_AnimatorController.controller";
        private const string PrefabOutputPath = "Assets/Prefabs/Characters/Kael.prefab";

        // ── Spritesheet layout (matches kael_spritesheet.png: 1024x1024, 6x4 grid) ──
        // Row 0 (top)  : Idle   — 4 frames (cols 0-3), cols 4-5 empty
        // Row 1        : Run    — 6 frames (cols 0-5)
        // Row 2        : Jump   — 4 frames (cols 0-3), cols 4-5 empty
        // Row 3 (bottom): Attack — 6 frames (cols 0-5)
        private const int Columns     = 6;
        private const int Rows        = 4;
        private const int FrameWidth  = 170;   // 1024 / 6  ≈ 170 px
        private const int FrameHeight = 256;   // 1024 / 4  = 256 px
        private const float FrameRate = 10f;   // FPS for animations

        [MenuItem("Tools/Kael/1 - Setup Kael Player (Run First!)")]
        public static void SetupKaelPlayer()
        {
            // ── 0. Validate spritesheet exists ─────────────────────────────────
            if (!File.Exists(Path.GetFullPath(SpritesheetPath)))
            {
                EditorUtility.DisplayDialog("Kael Setup Wizard",
                    $"Spritesheet not found at:\n{SpritesheetPath}\n\n" +
                    "Please copy kael_spritesheet.png into:\n" +
                    "Assets/Sprites/Characters/Kael/",
                    "OK");
                return;
            }

            // ── 1. Configure texture importer for sprite slicing ────────────────
            SliceSpritesheet();

            // ── 2. Create output directories ────────────────────────────────────
            EnsureDirectory(AnimClipsDir);
            EnsureDirectory("Assets/Prefabs/Characters");

            // ── 3. Load the sliced sprites ──────────────────────────────────────
            Sprite[] sprites = LoadAllSprites();
            if (sprites == null || sprites.Length == 0)
            {
                Debug.LogError("[KaelSetup] No sprites found in spritesheet after slicing. " +
                               "Check FrameWidth/FrameHeight constants in KaelSetupWizard.cs");
                return;
            }

            Debug.Log($"[KaelSetup] Loaded {sprites.Length} sprite(s) from spritesheet.");

            // ── 4. Build animation clips ─────────────────────────────────────────
            // Spritesheet layout:
            //   Row 0: Idle   frames  0- 3  (4 frames)
            //   Row 1: Run    frames  6-11  (6 frames)
            //   Row 2: Jump   frames 12-15  (4 frames)
            //   Row 3: Attack frames 18-23  (6 frames)
            AnimationClip idleClip   = BuildMultiFrameClip("Kael_Idle",   sprites,  0,  3, true);
            AnimationClip runClip    = BuildMultiFrameClip("Kael_Run",    sprites,  6, 11, true);
            AnimationClip jumpClip   = BuildMultiFrameClip("Kael_Jump",   sprites, 12, 13, false);
            AnimationClip fallClip   = BuildMultiFrameClip("Kael_Fall",   sprites, 14, 15, true);
            AnimationClip attackClip = BuildMultiFrameClip("Kael_Attack", sprites, 18, 23, false);

            SaveAsset(idleClip,   $"{AnimClipsDir}/Kael_Idle.anim");
            SaveAsset(runClip,    $"{AnimClipsDir}/Kael_Run.anim");
            SaveAsset(jumpClip,   $"{AnimClipsDir}/Kael_Jump.anim");
            SaveAsset(fallClip,   $"{AnimClipsDir}/Kael_Fall.anim");
            SaveAsset(attackClip, $"{AnimClipsDir}/Kael_Attack.anim");

            // ── 5. Build AnimatorController ──────────────────────────────────────
            AnimatorController controller = BuildAnimatorController(idleClip, runClip, jumpClip, fallClip, attackClip);

            // ── 6. Create the Kael prefab ────────────────────────────────────────
            CreateKaelPrefab(controller, sprites[0]);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Kael Setup Wizard",
                "✅  Setup complete!\n\n" +
                "• Sprites sliced\n" +
                "• Animation clips created\n" +
                "• AnimatorController wired\n" +
                "• Kael.prefab created\n\n" +
                "Drag 'Assets/Prefabs/Characters/Kael.prefab' into your scene.\n" +
                "Assign 'KaelMovementSettings' and 'KaelCombatSettings' assets in the Inspector.",
                "Done");
        }

        // ── Step 1: Slice ─────────────────────────────────────────────────────────
        private static void SliceSpritesheet()
        {
            TextureImporter importer = AssetImporter.GetAtPath(SpritesheetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType        = TextureImporterType.Sprite;
            importer.spriteImportMode   = SpriteImportMode.Multiple;
            importer.filterMode         = FilterMode.Point;     // pixel art — no bilinear blur
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled      = false;
            importer.alphaIsTransparency = true;
            importer.alphaSource        = TextureImporterAlphaSource.FromInput;

            TextureImporterSettings texSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(texSettings);
            texSettings.spriteMeshType = SpriteMeshType.Tight;
            importer.SetTextureSettings(texSettings);

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(SpritesheetPath);
            importer.spritesheet = GenerateSpriteMetaData(tex);

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log($"[KaelSetup] Spritesheet sliced into {Columns * Rows} cells ({FrameWidth}x{FrameHeight} each).");
        }

        private static SpriteMetaData[] GenerateSpriteMetaData(Texture2D tex)
        {
            // Use fixed grid constants that match the generated spritesheet
            var metas = new System.Collections.Generic.List<SpriteMetaData>();
            int index = 0;

            // Unity UV: row 0 = bottom of image. Our spritesheet row 0 = top (Idle).
            // So we iterate rows from top (Rows-1) to bottom (0) in UV space.
            for (int row = Rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Rect rect = new Rect(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
                    metas.Add(new SpriteMetaData
                    {
                        name      = $"kael_{index}",
                        rect      = rect,
                        pivot     = new Vector2(0.5f, 0.08f),  // pivot near feet
                        alignment = (int)SpriteAlignment.Custom,
                        border    = Vector4.zero,
                    });
                    index++;
                }
            }

            return metas.ToArray();
        }

        // ── Step 2: Load sprites ───────────────────────────────────────────────────
        private static Sprite[] LoadAllSprites()
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(SpritesheetPath);
            var sprites = new System.Collections.Generic.List<Sprite>();
            foreach (var a in assets)
                if (a is Sprite s) sprites.Add(s);

            sprites.Sort((a, b) =>
            {
                // Sort by the trailing index number: kael_0, kael_1 …
                int ia = ExtractIndex(a.name);
                int ib = ExtractIndex(b.name);
                return ia.CompareTo(ib);
            });

            return sprites.ToArray();
        }

        private static int ExtractIndex(string name)
        {
            string[] parts = name.Split('_');
            if (parts.Length > 0 && int.TryParse(parts[parts.Length - 1], out int n)) return n;
            return 0;
        }

        // ── Step 3: Build animation clips ─────────────────────────────────────────
        private static AnimationClip BuildSingleFrameClip(string clipName, Sprite[] sprites, int index, bool loop)
        {
            index = Mathf.Clamp(index, 0, sprites.Length - 1);
            return BuildMultiFrameClip(clipName, sprites, index, index, loop);
        }

        private static AnimationClip BuildMultiFrameClip(string clipName, Sprite[] sprites, int from, int to, bool loop)
        {
            var clip = new AnimationClip { name = clipName, frameRate = FrameRate };

            AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

            var binding = new EditorCurveBinding
            {
                type         = typeof(SpriteRenderer),
                path         = "",
                propertyName = "m_Sprite"
            };

            from = Mathf.Clamp(from, 0, sprites.Length - 1);
            to   = Mathf.Clamp(to,   0, sprites.Length - 1);
            int frameCount = to - from + 1;

            var keyframes = new ObjectReferenceKeyframe[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe
                {
                    time  = i / FrameRate,
                    value = sprites[from + i]
                };
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
            return clip;
        }

        // ── Step 4: Build AnimatorController ──────────────────────────────────────
        private static AnimatorController BuildAnimatorController(
            AnimationClip idleClip, AnimationClip runClip,
            AnimationClip jumpClip, AnimationClip fallClip,
            AnimationClip attackClip)
        {
            // Delete old controller so we always build fresh
            AssetDatabase.DeleteAsset(AnimControllerPath);

            AnimatorController ctrl = AnimatorController.CreateAnimatorControllerAtPath(AnimControllerPath);
            AnimatorStateMachine sm = ctrl.layers[0].stateMachine;

            // ── Parameters ─────────────────────────────────────────────────────
            ctrl.AddParameter("isIdle",      AnimatorControllerParameterType.Bool);
            ctrl.AddParameter("isRunning",   AnimatorControllerParameterType.Bool);
            ctrl.AddParameter("isJumping",   AnimatorControllerParameterType.Bool);
            ctrl.AddParameter("isFalling",   AnimatorControllerParameterType.Bool);
            ctrl.AddParameter("isWallSliding",AnimatorControllerParameterType.Bool);
            ctrl.AddParameter("isAttacking", AnimatorControllerParameterType.Bool);
            ctrl.AddParameter("comboIndex",  AnimatorControllerParameterType.Int);

            // ── States ──────────────────────────────────────────────────────────
            AnimatorState idleState   = sm.AddState("Idle",      new Vector3(200, 0));
            idleState.motion = idleClip;

            AnimatorState runState    = sm.AddState("Run",       new Vector3(200, 60));
            runState.motion = runClip;

            AnimatorState jumpState   = sm.AddState("Jump",      new Vector3(200, 120));
            jumpState.motion = jumpClip;

            AnimatorState fallState   = sm.AddState("Fall",      new Vector3(200, 180));
            fallState.motion = fallClip;

            AnimatorState wallSlide   = sm.AddState("WallSlide", new Vector3(200, 240));
            wallSlide.motion = fallClip;   // reuse fall clip for wall slide

            AnimatorState attackState = sm.AddState("Attack",    new Vector3(400, 0));
            attackState.motion = attackClip;

            // ── Default state ────────────────────────────────────────────────────
            sm.defaultState = idleState;

            // ── Transitions from Idle ──────────────────────────────────────────
            AddBoolTransition(idleState, runState,    ctrl, "isRunning",    true,  0f, 0f);
            AddBoolTransition(idleState, jumpState,   ctrl, "isJumping",    true,  0f, 0f);
            AddBoolTransition(idleState, fallState,   ctrl, "isFalling",    true,  0f, 0f);
            AddBoolTransition(idleState, attackState, ctrl, "isAttacking",  true,  0f, 0f);

            // ── Transitions from Run ───────────────────────────────────────────
            AddBoolTransition(runState, idleState,   ctrl, "isRunning",   false, 0f, 0f);
            AddBoolTransition(runState, jumpState,   ctrl, "isJumping",   true,  0f, 0f);
            AddBoolTransition(runState, fallState,   ctrl, "isFalling",   true,  0f, 0f);
            AddBoolTransition(runState, attackState, ctrl, "isAttacking", true,  0f, 0f);

            // ── Transitions from Jump ──────────────────────────────────────────
            AddBoolTransition(jumpState, fallState,  ctrl, "isFalling",  true,  0f, 0f);
            AddBoolTransition(jumpState, idleState,  ctrl, "isJumping",  false, 0f, 0.1f);

            // ── Transitions from Fall ──────────────────────────────────────────
            AddBoolTransition(fallState, idleState,    ctrl, "isFalling",    false, 0f, 0f);
            AddBoolTransition(fallState, wallSlide,    ctrl, "isWallSliding",true,  0f, 0f);

            // ── Transitions from WallSlide ─────────────────────────────────────
            AddBoolTransition(wallSlide, fallState,   ctrl, "isWallSliding", false, 0f, 0f);
            AddBoolTransition(wallSlide, attackState, ctrl, "isAttacking",   true,  0f, 0f);

            // ── Transitions from Attack ────────────────────────────────────────
            // Attack loops back to idle/run when isAttacking becomes false
            AddBoolTransition(attackState, idleState, ctrl, "isAttacking", false, 0f, 0f);

            AssetDatabase.SaveAssets();
            Debug.Log("[KaelSetup] AnimatorController built: " + AnimControllerPath);
            return ctrl;
        }

        private static void AddBoolTransition(
            AnimatorState from, AnimatorState to,
            AnimatorController ctrl,
            string param, bool value,
            float exitTime, float duration)
        {
            AnimatorStateTransition t = from.AddTransition(to);
            t.hasExitTime    = exitTime > 0f;
            t.exitTime       = exitTime;
            t.duration       = duration;
            t.hasFixedDuration = true;
            t.AddCondition(value
                ? AnimatorConditionMode.If
                : AnimatorConditionMode.IfNot,
                0f, param);
        }

        // ── Step 5: Create Prefab ──────────────────────────────────────────────────
        private static void CreateKaelPrefab(AnimatorController controller, Sprite idleSprite)
        {
            // Root
            var root = new GameObject("Kael");

            // ── Rigidbody2D ──
            var rb = root.AddComponent<Rigidbody2D>();
            rb.gravityScale      = 1f;
            rb.freezeRotation    = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation     = RigidbodyInterpolation2D.Interpolate;

            // ── CapsuleCollider2D ──
            var col = root.AddComponent<CapsuleCollider2D>();
            col.size   = new Vector2(0.6f, 1.4f);
            col.offset = new Vector2(0f, 0.7f);

            // ── SpriteRenderer ──
            var sr = root.AddComponent<SpriteRenderer>();
            sr.sprite  = idleSprite;
            sr.sortingLayerName = "Characters";
            sr.sortingOrder     = 0;

            // ── Animator ──
            var anim = root.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller;
            anim.updateMode = AnimatorUpdateMode.Normal;

            // ── PlayerInput ──
            root.AddComponent<PlayerInput>();

            // ── PlayerController (last, because Awake needs other components) ──
            root.AddComponent<PlayerController>();

            // ── Attack Hitbox child ──
            var hitboxGO = new GameObject("AttackHitbox");
            hitboxGO.transform.SetParent(root.transform, false);
            hitboxGO.transform.localPosition = new Vector3(0.9f, 0.7f, 0f);
            hitboxGO.SetActive(false); // activated by AnimationEvent or directly by AttackState

            // ── Save as prefab ──
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabOutputPath);
            Object.DestroyImmediate(root);

            Debug.Log("[KaelSetup] Prefab saved: " + PrefabOutputPath);
            Selection.activeObject = prefab;
        }

        // ── Utilities ─────────────────────────────────────────────────────────────
        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folder = Path.GetFileName(path);
                EnsureDirectory(parent);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }

        private static void SaveAsset(Object asset, string path)
        {
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(asset, path);
        }

        // ── Quick helper: re-run only the prefab step ──────────────────────────────
        [MenuItem("Tools/Kael/2 - Rebuild Prefab Only")]
        public static void RebuildPrefabOnly()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimControllerPath);
            if (controller == null)
            {
                Debug.LogError("[KaelSetup] No AnimatorController found. Run step 1 first.");
                return;
            }

            Sprite[] sprites = LoadAllSprites();
            if (sprites == null || sprites.Length == 0)
            {
                Debug.LogError("[KaelSetup] No sprites found. Check the spritesheet path.");
                return;
            }

            EnsureDirectory("Assets/Prefabs/Characters");
            CreateKaelPrefab(controller, sprites[0]);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
