using System.Numerics;
using NonSensibleSoccer.Game.Models;

namespace NonSensibleSoccer.Game.Systems;

internal sealed class CpuController
{
    public void UpdateCpuTeam(Team cpuTeam, Team humanTeam, Ball ball, PhysicsGameplay physics, float dt, RectangleF pitch)
    {
        var chaser = cpuTeam.Players
            .OrderBy(p => Vector2.DistanceSquared(p.Position, ball.Position))
            .First();

        cpuTeam.SetActivePlayer(chaser);

        foreach (var player in cpuTeam.Players)
        {
            Vector2 target;

            if (ReferenceEquals(player, chaser))
            {
                target = ball.Owner is not null && ball.Owner.IsHumanTeam
                    ? ball.Owner.Position
                    : ball.Position;
            }
            else
            {
                var ballBias = new Vector2((ball.Position.X - player.HomePosition.X) * 0.2f, (ball.Position.Y - player.HomePosition.Y) * 0.2f);
                target = player.HomePosition + ballBias;
            }

            var dir = target - player.Position;
            if (dir.LengthSquared() > 0)
            {
                dir = Vector2.Normalize(dir);
            }

            physics.MovePlayer(player, dir, dt, pitch, physics.CpuPlayerSpeed);
        }

        if (ReferenceEquals(ball.Owner, chaser))
        {
            var goalTarget = new Vector2(pitch.Left, pitch.Top + pitch.Height * 0.5f);
            var toGoal = goalTarget - chaser.Position;
            var distanceToGoal = toGoal.Length();

            if (distanceToGoal < 220f)
            {
                physics.KickBall(ball, toGoal, physics.ShotPower * 0.95f);
                return;
            }

            var passTarget = cpuTeam.Players
                .Where(p => !ReferenceEquals(p, chaser))
                .OrderByDescending(p => p.Position.X)
                .FirstOrDefault();

            if (passTarget is not null && Random.Shared.NextSingle() < 0.02f)
            {
                physics.KickBall(ball, passTarget.Position - chaser.Position, physics.PassPower);
            }
        }
    }
}
