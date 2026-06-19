using System.Numerics;
using NonSensibleSoccer.Game.Models;

namespace NonSensibleSoccer.Game.Systems;

internal sealed class PhysicsGameplay
{
    private const float PickupTieMargin = 2f;

    public float PlayerSpeed { get; set; } = 190f;
    public float CpuPlayerSpeed { get; set; } = 178f;
    public float BallFriction { get; set; } = 0.985f;
    public float PickupRadius { get; set; } = 16f;
    public float PassPower { get; set; } = 300f;
    public float ShotPower { get; set; } = 420f;

    public void MovePlayer(Player player, Vector2 direction, float dt, RectangleF pitch, float speed)
    {
        player.Velocity = direction * speed;
        player.Position += player.Velocity * dt;
        player.Position = ClampToPitch(player.Position, player.Radius, pitch);
    }

    public void UpdateBall(Ball ball, float dt, RectangleF pitch)
    {
        if (ball.PickupCooldownSeconds > 0f)
        {
            ball.PickupCooldownSeconds = MathF.Max(0f, ball.PickupCooldownSeconds - dt);
        }

        if (ball.Owner is not null)
        {
            ball.Position = ball.Owner.Position + new Vector2(ball.Owner.Radius + 3f, 0f);
            ball.Velocity = Vector2.Zero;
            return;
        }

        ball.Position += ball.Velocity * dt;
        ball.Velocity *= BallFriction;

        if (MathF.Abs(ball.Velocity.X) < 5f) ball.Velocity = new Vector2(0f, ball.Velocity.Y);
        if (MathF.Abs(ball.Velocity.Y) < 5f) ball.Velocity = new Vector2(ball.Velocity.X, 0f);

        if (ball.Position.X - ball.Radius < pitch.Left)
        {
            ball.Position = new Vector2(pitch.Left + ball.Radius, ball.Position.Y);
            ball.Velocity = new Vector2(-ball.Velocity.X * 0.75f, ball.Velocity.Y);
        }

        if (ball.Position.X + ball.Radius > pitch.Right)
        {
            ball.Position = new Vector2(pitch.Right - ball.Radius, ball.Position.Y);
            ball.Velocity = new Vector2(-ball.Velocity.X * 0.75f, ball.Velocity.Y);
        }

        if (ball.Position.Y - ball.Radius < pitch.Top)
        {
            ball.Position = new Vector2(ball.Position.X, pitch.Top + ball.Radius);
            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y * 0.75f);
        }

        if (ball.Position.Y + ball.Radius > pitch.Bottom)
        {
            ball.Position = new Vector2(ball.Position.X, pitch.Bottom - ball.Radius);
            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y * 0.75f);
        }
    }

    public bool TryPickupBall(Ball ball, IEnumerable<Player> players)
    {
        if (ball.Owner is not null || ball.PickupCooldownSeconds > 0f)
        {
            return false;
        }

        var candidates = players
            .Select(p => new { Player = p, Dist = Vector2.Distance(p.Position, ball.Position) })
            .Where(x => x.Dist <= PickupRadius + x.Player.Radius)
            .OrderBy(x => x.Dist)
            .Take(2)
            .ToArray();

        if (candidates.Length == 0)
        {
            return false;
        }

        if (candidates.Length > 1 && MathF.Abs(candidates[0].Dist - candidates[1].Dist) <= PickupTieMargin)
        {
            return false;
        }

        var winner = candidates[0].Player;
        if (ReferenceEquals(winner, ball.LastOwner))
        {
            return false;
        }

        ball.Owner = winner;
        ball.LastOwner = winner;
        return true;
    }

    public void KickBall(Ball ball, Vector2 direction, float power)
    {
        if (ball.Owner is null)
        {
            return;
        }

        var previousOwner = ball.Owner;

        var normalized = direction.LengthSquared() > 0 ? Vector2.Normalize(direction) : Vector2.UnitX;
        ball.Position = previousOwner.Position + normalized * (previousOwner.Radius + ball.Radius + 2f);
        ball.Velocity = normalized * power;
        ball.Owner = null;
        ball.LastOwner = previousOwner;
        ball.PickupCooldownSeconds = 0.08f;
    }

    private static Vector2 ClampToPitch(Vector2 pos, float radius, RectangleF pitch)
    {
        return new Vector2(
            Math.Clamp(pos.X, pitch.Left + radius, pitch.Right - radius),
            Math.Clamp(pos.Y, pitch.Top + radius, pitch.Bottom - radius));
    }
}
