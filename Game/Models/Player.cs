using System.Numerics;

namespace NonSensibleSoccer.Game.Models;

internal sealed class Player
{
    public required string Name { get; init; }
    public required Vector2 HomePosition { get; init; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public bool IsHumanTeam { get; init; }
    public bool IsGoalkeeper { get; init; }
    public bool IsActive { get; set; }

    public float Radius => 10f;
}
