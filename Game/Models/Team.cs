namespace NonSensibleSoccer.Game.Models;

internal sealed class Team
{
    public required string Name { get; init; }
    public required List<Player> Players { get; init; }
    public bool IsHuman { get; init; }
    public int Score { get; set; }

    public Player ActivePlayer => Players.First(p => p.IsActive);

    public void SetActivePlayer(Player player)
    {
        foreach (var p in Players)
        {
            p.IsActive = ReferenceEquals(p, player);
        }
    }
}
