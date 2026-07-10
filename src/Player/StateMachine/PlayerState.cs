using Godot;

namespace Player.StateMachine
{
    /// <summary>
    /// Abstract base class for all player states.
    /// Godot port: Time.time → elapsed computed via Engine ticks,
    /// Animator bools → AnimationPlayer play calls.
    /// </summary>
    public abstract class PlayerState
    {
        protected PlayerController Player;
        protected PlayerStateMachine StateMachine;
        protected string AnimBoolName;

        /// <summary>Engine time (seconds) when this state was entered.</summary>
        protected double StartTime;

        public PlayerState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
        {
            Player       = player;
            StateMachine = stateMachine;
            AnimBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            StartTime = Time.GetTicksMsec() / 1000.0;
            if (!string.IsNullOrEmpty(AnimBoolName))
                Player.PlayAnimation(AnimBoolName);
        }

        public virtual void Exit() { }

        /// <summary>Called every frame (from _Process). Read input, check transitions.</summary>
        public virtual void LogicUpdate() { }

        /// <summary>Called every physics tick (from _PhysicsProcess). Apply forces, velocity.</summary>
        public virtual void PhysicsUpdate() { }

        /// <summary>Seconds elapsed since this state was entered.</summary>
        protected double Elapsed => Time.GetTicksMsec() / 1000.0 - StartTime;
    }
}
