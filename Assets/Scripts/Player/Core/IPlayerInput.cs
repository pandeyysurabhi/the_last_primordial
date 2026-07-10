using UnityEngine;

namespace Player
{
    /// <summary>
    /// Defines the input requirements for the 2D Character Controller.
    /// Decoupling input from the physics controller allows for easy swapping of input backends
    /// (legacy Input Manager, New Input System, AI, replay, etc.)
    /// </summary>
    public interface IPlayerInput
    {
        /// <summary>Raw horizontal input value, typically -1 to 1.</summary>
        float Horizontal { get; }

        /// <summary>Raw vertical input value, typically -1 to 1.</summary>
        float Vertical { get; }

        // ── Jump ──────────────────────────────────────────────────────────────
        /// <summary>True during the frame the jump button was pressed.</summary>
        bool JumpDown { get; }

        /// <summary>True during the frame the jump button was released.</summary>
        bool JumpUp { get; }

        /// <summary>True as long as the jump button is held.</summary>
        bool JumpHeld { get; }

        /// <summary>True if a jump input is currently buffered.</summary>
        bool HasJumpBuffered { get; }

        /// <summary>Resets the jump buffer so it isn't consumed multiple times.</summary>
        void ConsumeJumpBuffer();

        // ── Attack ────────────────────────────────────────────────────────────
        /// <summary>True during the frame the attack button was pressed.</summary>
        bool AttackDown { get; }
    }
}
