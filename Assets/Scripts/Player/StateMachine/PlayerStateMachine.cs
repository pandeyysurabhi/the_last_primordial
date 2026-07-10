namespace Player.StateMachine
{
    /// <summary>
    /// Manages state transitions and maintains a reference to the active state.
    /// </summary>
    public class PlayerStateMachine
    {
        /// <summary>
        /// The currently active player state.
        /// </summary>
        public PlayerState CurrentState { get; private set; }

        /// <summary>
        /// Initializes the state machine with a starting state.
        /// </summary>
        public void Initialize(PlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        /// <summary>
        /// Safely exits the active state and enters a new state.
        /// </summary>
        public void ChangeState(PlayerState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
