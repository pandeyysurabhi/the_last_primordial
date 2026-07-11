namespace Player
{
    /// <summary>
    /// Contract that any input reader must fulfill.
    /// Decouples states from concrete input implementation.
    /// </summary>
    public interface IPlayerInput
    {
        float Horizontal    { get; }
        float Vertical      { get; }
        bool  JumpDown      { get; }
        bool  JumpUp        { get; }
        bool  JumpHeld      { get; }
        bool  AttackDown    { get; }
        bool  PhaseShiftDown { get; }
        bool  TimeFreezeDown { get; }
        bool  TetherHeld    { get; }
        bool  HasJumpBuffered { get; }
        void  ConsumeJumpBuffer();
    }
}
