using System.Numerics;
using NonSensibleSoccer.Game.Models;

namespace NonSensibleSoccer.Game.Systems;

internal sealed class RulesSystem
{
    public Team? TryDetectGoal(Ball ball, Team humanTeam, Team cpuTeam, RectangleF pitch)
    {
        var goalTop = pitch.Top + pitch.Height * 0.35f;
        var goalBottom = pitch.Bottom - pitch.Height * 0.35f;
        var inGoalMouth = ball.Position.Y >= goalTop && ball.Position.Y <= goalBottom;

        if (!inGoalMouth)
        {
            return null;
        }

        if (ball.Position.X <= pitch.Left + 3f)
        {
            return humanTeam;
        }

        if (ball.Position.X >= pitch.Right - 3f)
        {
            return cpuTeam;
        }

        return null;
    }

    public void ResetKickoff(Ball ball, Team humanTeam, Team cpuTeam, RectangleF pitch, MatchState matchState)
    {
        var center = new Vector2(pitch.Left + pitch.Width * 0.5f, pitch.Top + pitch.Height * 0.5f);

        foreach (var player in humanTeam.Players)
        {
            player.Position = player.HomePosition;
            player.Velocity = Vector2.Zero;
        }

        foreach (var player in cpuTeam.Players)
        {
            player.Position = player.HomePosition;
            player.Velocity = Vector2.Zero;
        }

        var humanKickoffPlayer = humanTeam.Players
            .OrderBy(p => Vector2.DistanceSquared(p.Position, center))
            .First();

        humanTeam.SetActivePlayer(humanKickoffPlayer);
        cpuTeam.SetActivePlayer(cpuTeam.Players[0]);

        ball.Owner = humanKickoffPlayer;
        ball.LastOwner = humanKickoffPlayer;
        ball.PickupCooldownSeconds = 0f;
        ball.Position = center;
        ball.Velocity = Vector2.Zero;

        matchState.IsKickoff = true;
        matchState.TimeSinceKickoff = 0f;
        matchState.Message = "Kickoff";
    }

    public bool CheckGameOver(Team humanTeam, Team cpuTeam, MatchState state)
    {
        if (humanTeam.Score >= state.TargetGoals)
        {
            state.IsGameOver = true;
            state.Message = "You win! Press Enter to restart.";
            return true;
        }

        if (cpuTeam.Score >= state.TargetGoals)
        {
            state.IsGameOver = true;
            state.Message = "CPU wins! Press Enter to restart.";
            return true;
        }

        return false;
    }
}
