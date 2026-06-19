using System.Numerics;
using NonSensibleSoccer.Game.Models;
using NonSensibleSoccer.Game.Systems;

namespace NonSensibleSoccer.Game;

internal sealed class GameWorld
{
    private const float CpuSpeedBeginner = 150f;
    private const float CpuSpeedNormal = 168f;

    private readonly InputController _input;
    private readonly PhysicsGameplay _physics = new();
    private readonly CpuController _cpu = new();
    private readonly RulesSystem _rules = new();

    public Team HumanTeam { get; }
    public Team CpuTeam { get; }
    public Ball Ball { get; }
    public MatchState MatchState { get; } = new();
    public RectangleF Pitch { get; private set; }

    public GameWorld(InputController input, RectangleF pitch)
    {
        _input = input;
        Pitch = pitch;

        HumanTeam = new Team
        {
            Name = "Human",
            IsHuman = true,
            Players = BuildTeam(isHuman: true, pitch)
        };

        CpuTeam = new Team
        {
            Name = "CPU",
            IsHuman = false,
            Players = BuildTeam(isHuman: false, pitch)
        };

        HumanTeam.SetActivePlayer(HumanTeam.Players[0]);
        CpuTeam.SetActivePlayer(CpuTeam.Players[0]);

        Ball = new Ball
        {
            Position = Center(pitch),
            Velocity = Vector2.Zero,
            Owner = null
        };

        ApplyDifficulty(MatchState.Difficulty, showMessage: false);
        MatchState.ResetTransientState();
        _rules.ResetKickoff(Ball, HumanTeam, CpuTeam, Pitch, MatchState);
    }

    public void ResizePitch(RectangleF pitch)
    {
        Pitch = pitch;
    }

    public void Restart()
    {
        HumanTeam.Score = 0;
        CpuTeam.Score = 0;
        MatchState.ResetTransientState();
        _rules.ResetKickoff(Ball, HumanTeam, CpuTeam, Pitch, MatchState);
    }

    public void Update(float dt)
    {
        HandleDifficultyInput();

        if (MatchState.IsGameOver)
        {
            if (_input.ConsumeRestartPressed())
            {
                Restart();
            }

            return;
        }

        MatchState.TimeSinceKickoff += dt;
        if (MatchState.IsKickoff && MatchState.TimeSinceKickoff > 0.8f)
        {
            MatchState.IsKickoff = false;
            MatchState.Message = "";
        }

        HandleHumanInput(dt);
        _cpu.UpdateCpuTeam(CpuTeam, HumanTeam, Ball, _physics, dt, Pitch);

        _physics.TryPickupBall(Ball, HumanTeam.Players.Concat(CpuTeam.Players));
        _physics.UpdateBall(Ball, dt, Pitch);

        var scorer = _rules.TryDetectGoal(Ball, HumanTeam, CpuTeam, Pitch);
        if (scorer is not null)
        {
            scorer.Score++;
            MatchState.Message = scorer.IsHuman ? "Goal for you!" : "Goal for CPU!";
            var isOver = _rules.CheckGameOver(HumanTeam, CpuTeam, MatchState);
            if (!isOver)
            {
                _rules.ResetKickoff(Ball, HumanTeam, CpuTeam, Pitch, MatchState);
            }
        }
    }

    private void HandleDifficultyInput()
    {
        if (_input.ConsumeBeginnerPressed())
        {
            ApplyDifficulty(DifficultyLevel.Beginner, showMessage: true);
            return;
        }

        if (_input.ConsumeNormalPressed())
        {
            ApplyDifficulty(DifficultyLevel.Normal, showMessage: true);
        }
    }

    private void ApplyDifficulty(DifficultyLevel difficulty, bool showMessage)
    {
        MatchState.Difficulty = difficulty;
        _physics.CpuPlayerSpeed = difficulty switch
        {
            DifficultyLevel.Beginner => CpuSpeedBeginner,
            _ => CpuSpeedNormal
        };

        if (showMessage)
        {
            MatchState.Message = difficulty == DifficultyLevel.Beginner
                ? "Difficulty: Beginner"
                : "Difficulty: Normal";
            MatchState.TimeSinceKickoff = 0f;
            MatchState.IsKickoff = true;
        }
    }

    private void HandleHumanInput(float dt)
    {
        var active = HumanTeam.ActivePlayer;
        var moveDir = _input.MovementDirection;
        _physics.MovePlayer(active, moveDir, dt, Pitch, _physics.PlayerSpeed);

        if (_input.ConsumeSwitchPressed())
        {
            SwitchToNearestTeammateToBall();
        }

        if (ReferenceEquals(Ball.Owner, active))
        {
            if (_input.ConsumePassPressed())
            {
                var target = HumanTeam.Players
                    .Where(p => !ReferenceEquals(p, active))
                    .OrderBy(p => Vector2.DistanceSquared(p.Position, Ball.Position))
                    .First();

                _physics.KickBall(Ball, target.Position - active.Position, _physics.PassPower);
            }
            else if (_input.ConsumeShootPressed())
            {
                var target = new Vector2(Pitch.Right, Pitch.Top + Pitch.Height * 0.5f);
                _physics.KickBall(Ball, target - active.Position, _physics.ShotPower);
            }
        }

        foreach (var teammate in HumanTeam.Players.Where(p => !ReferenceEquals(p, active)))
        {
            var homeDrift = teammate.HomePosition - teammate.Position;
            var dir = homeDrift.LengthSquared() > 1f ? Vector2.Normalize(homeDrift) : Vector2.Zero;
            _physics.MovePlayer(teammate, dir, dt, Pitch, _physics.PlayerSpeed * 0.35f);
        }
    }

    private void SwitchToNearestTeammateToBall()
    {
        var nearest = HumanTeam.Players
            .OrderBy(p => Vector2.DistanceSquared(p.Position, Ball.Position))
            .First();

        HumanTeam.SetActivePlayer(nearest);
    }

    private static Vector2 Center(RectangleF rect) => new(rect.Left + rect.Width * 0.5f, rect.Top + rect.Height * 0.5f);

    private static List<Player> BuildTeam(bool isHuman, RectangleF pitch)
    {
        var xBase = isHuman ? pitch.Left + pitch.Width * 0.24f : pitch.Left + pitch.Width * 0.76f;
        var mirror = isHuman ? 1f : -1f;

        var template = new[]
        {
            new Vector2(-120f, 0f),
            new Vector2(-30f, -90f),
            new Vector2(-30f, 90f),
            new Vector2(70f, -50f),
            new Vector2(70f, 50f)
        };

        var players = new List<Player>(template.Length);
        for (var i = 0; i < template.Length; i++)
        {
            var offset = new Vector2(template[i].X * mirror, template[i].Y);
            var home = new Vector2(xBase + offset.X, pitch.Top + pitch.Height * 0.5f + offset.Y);
            players.Add(new Player
            {
                Name = (isHuman ? "H" : "C") + (i + 1),
                HomePosition = home,
                Position = home,
                Velocity = Vector2.Zero,
                IsHumanTeam = isHuman,
                IsGoalkeeper = i == 0,
                IsActive = i == 0
            });
        }

        return players;
    }
}
