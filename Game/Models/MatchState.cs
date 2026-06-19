namespace NonSensibleSoccer.Game.Models;

internal sealed class MatchState
{
    public int TargetGoals { get; init; } = 3;
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public bool IsKickoff { get; set; }
    public bool IsGameOver { get; set; }
    public string Message { get; set; } = "";
    public float TimeSinceKickoff { get; set; }

    public void ResetTransientState()
    {
        IsKickoff = true;
        IsGameOver = false;
        TimeSinceKickoff = 0f;
    }
}
