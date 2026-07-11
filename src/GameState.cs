namespace TheLastPrimordial
{
    /// <summary>
    /// Tracks the player's progress and trigger states across the opening chapter.
    /// </summary>
    public static class GameState
    {
        public static bool HasCottageKey { get; set; } = false;
        public static bool CottageInvestigated { get; set; } = false;
        public static bool TimeFrozen { get; set; } = false;
        public static bool HasBlackSword { get; set; } = false;
        public static bool TalkedToGideon { get; set; } = false;
        public static bool TalkedToVala { get; set; } = false;
        public static bool TalkedToLia { get; set; } = false;

        public static void Reset()
        {
            HasCottageKey = false;
            CottageInvestigated = false;
            TimeFrozen = false;
            HasBlackSword = false;
            TalkedToGideon = false;
            TalkedToVala = false;
            TalkedToLia = false;
        }
    }
}
