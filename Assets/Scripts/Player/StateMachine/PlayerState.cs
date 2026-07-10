using UnityEngine;

namespace Player.StateMachine
{
    /// <summary>
    /// Abstract base class for all movement states.
    /// Encapsulates state-specific logic, physics updates, and transitions.
    /// Animation is driven by bool parameters matching animBoolName in the AnimatorController.
    /// </summary>
    public abstract class PlayerState
    {
        protected PlayerController player;
        protected PlayerStateMachine stateMachine;

        /// <summary>
        /// The Animator bool parameter name that this state owns.
        /// Set to true on Enter and false on Exit, driving the AnimatorController blends.
        /// </summary>
        protected string animBoolName;

        /// <summary>
        /// Time.time when this state was entered. Useful for state duration checks.
        /// </summary>
        protected float startTime;

        public PlayerState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
        {
            this.player = player;
            this.stateMachine = stateMachine;
            this.animBoolName = animBoolName;
        }

        /// <summary>
        /// Logic executed when first entering this state.
        /// </summary>
        public virtual void Enter()
        {
            startTime = Time.time;
            if (player.Animator != null && !string.IsNullOrEmpty(animBoolName))
                player.Animator.SetBool(animBoolName, true);
        }

        /// <summary>
        /// Logic executed when transitioning out of this state.
        /// </summary>
        public virtual void Exit()
        {
            if (player.Animator != null && !string.IsNullOrEmpty(animBoolName))
                player.Animator.SetBool(animBoolName, false);
        }

        /// <summary>
        /// Executed every frame in Update(). Good for reading input and state transitions.
        /// </summary>
        public virtual void LogicUpdate() { }

        /// <summary>
        /// Executed every physics step in FixedUpdate(). Good for applying forces and velocities.
        /// </summary>
        public virtual void PhysicsUpdate() { }
    }
}
