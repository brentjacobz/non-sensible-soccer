using System.Numerics;

namespace NonSensibleSoccer.Game.Models;

internal sealed class Ball
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Player? Owner { get; set; }
    public Player? LastOwner { get; set; }
    public float PickupCooldownSeconds { get; set; }
    public float Radius => 5f;
}
