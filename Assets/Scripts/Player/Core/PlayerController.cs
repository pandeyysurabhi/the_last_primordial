using UnityEngine;
using Player.StateMachine;
using Player.StateMachine.States;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovementSettings settings;
        [SerializeField] private KaelCombatSettings combatSettings;

        // Public properties for states to access
        public PlayerMovementSettings Settings => settings;
        public KaelCombatSettings CombatSettings => combatSettings;
        public Rigidbody2D Rb { get; private set; }
        public Collider2D Collider { get; private set; }
        public Animator Animator { get; private set; }
        public IPlayerInput InputReader { get; private set; }
        public PlayerStateMachine StateMachine { get; private set; }

        // ── States ──────────────────────────────────────────────────────────────
        public PlayerIdleState    IdleState      { get; private set; }
        public PlayerRunState     RunState       { get; private set; }
        public PlayerJumpState    JumpState      { get; private set; }
        public PlayerFallState    FallState      { get; private set; }
        public PlayerWallSlideState WallSlideState { get; private set; }
        public PlayerWallJumpState  WallJumpState  { get; private set; }
        public PlayerAttackState  AttackState    { get; private set; }

        // ── Collision state ──────────────────────────────────────────────────────
        public bool IsGrounded    { get; private set; }
        public bool IsOnWall      { get; private set; }
        public int  WallDirection { get; private set; }   // 1=Right  -1=Left  0=None
        public int  FacingDirection { get; private set; } = 1; // 1=Right  -1=Left

        // ── Attack hitbox ────────────────────────────────────────────────────────
        /// <summary>Hitbox that's activated during an attack. Assign in Inspector.</summary>
        [Header("Combat")]
        [SerializeField] private Transform attackHitboxPivot;

        public Transform AttackHitboxPivot => attackHitboxPivot;

        // ── Control lock ─────────────────────────────────────────────────────────
        private float _controlLockTimer;
        public bool CanMove => _controlLockTimer <= 0f;

        // ── Lifecycle ────────────────────────────────────────────────────────────
        private void Awake()
        {
            Rb       = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            Animator = GetComponent<Animator>();

            InputReader = GetComponent<IPlayerInput>();
            if (InputReader == null)
                Debug.LogError("[PlayerController] Requires a component implementing IPlayerInput on the same GameObject.", this);

            if (combatSettings == null)
                Debug.LogWarning("[PlayerController] No KaelCombatSettings assigned. Attack state will use defaults.", this);

            // ── Build all states ──
            StateMachine    = new PlayerStateMachine();
            IdleState       = new PlayerIdleState(this, StateMachine, "isIdle");
            RunState        = new PlayerRunState(this, StateMachine, "isRunning");
            JumpState       = new PlayerJumpState(this, StateMachine, "isJumping");
            FallState       = new PlayerFallState(this, StateMachine, "isFalling");
            WallSlideState  = new PlayerWallSlideState(this, StateMachine, "isWallSliding");
            WallJumpState   = new PlayerWallJumpState(this, StateMachine, "isJumping");
            AttackState     = new PlayerAttackState(this, StateMachine, "isAttacking");
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            if (_controlLockTimer > 0f) _controlLockTimer -= Time.deltaTime;
            StateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            PerformCollisionChecks();
            StateMachine.CurrentState.PhysicsUpdate();
        }

        // ── Collision & Movement ─────────────────────────────────────────────────
        private void PerformCollisionChecks()
        {
            Vector2 position = Rb.position;

            // Ground
            IsGrounded = Physics2D.OverlapBox(
                position + settings.groundCheckOffset,
                settings.groundCheckSize, 0f, settings.groundLayer);

            // Walls
            bool touchRight = Physics2D.OverlapBox(
                position + settings.wallCheckOffset,
                settings.wallCheckSize, 0f, settings.wallLayer);

            bool touchLeft = Physics2D.OverlapBox(
                position + new Vector2(-settings.wallCheckOffset.x, settings.wallCheckOffset.y),
                settings.wallCheckSize, 0f, settings.wallLayer);

            if (touchRight)      { IsOnWall = true;  WallDirection =  1; }
            else if (touchLeft)  { IsOnWall = true;  WallDirection = -1; }
            else                 { IsOnWall = false; WallDirection =  0; }
        }

        public void LockMovementControl(float duration) => _controlLockTimer = duration;

        public void SetVelocity(float x, float y)  => Rb.velocity = new Vector2(x, y);
        public void SetVelocityX(float x)           => Rb.velocity = new Vector2(x, Rb.velocity.y);
        public void SetVelocityY(float y)           => Rb.velocity = new Vector2(Rb.velocity.x, y);

        public void CheckIfShouldFlip(float horizontalInput)
        {
            if      (horizontalInput > 0 && FacingDirection == -1) Flip();
            else if (horizontalInput < 0 && FacingDirection ==  1) Flip();
        }

        private void Flip()
        {
            FacingDirection *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        // ── Editor Gizmos ────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            if (settings == null) return;

            Vector2 pos = (Vector2)transform.position;

            // Ground check — green when grounded, red when not
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(pos + settings.groundCheckOffset, settings.groundCheckSize);

            // Wall checks — cyan
            Gizmos.color = IsOnWall ? Color.green : Color.cyan;
            Gizmos.DrawWireCube(pos + settings.wallCheckOffset, settings.wallCheckSize);
            Gizmos.DrawWireCube(pos + new Vector2(-settings.wallCheckOffset.x, settings.wallCheckOffset.y), settings.wallCheckSize);

            // Attack hitbox pivot — yellow
            if (attackHitboxPivot != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(attackHitboxPivot.position, 0.15f);
            }
        }
    }
}
