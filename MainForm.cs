using System.Diagnostics;
using NonSensibleSoccer.Game;
using NonSensibleSoccer.Game.Rendering;
using NonSensibleSoccer.Game.Systems;
using Timer = System.Windows.Forms.Timer;

namespace NonSensibleSoccer;

internal sealed class MainForm : Form
{
    private readonly InputController _input = new();
    private readonly GameRenderer _renderer = new();
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly Timer _timer;

    private GameWorld _world;
    private long _lastTicks;

    public MainForm()
    {
        Text = "Non-Sensible Soccer (Prototype)";
        ClientSize = new Size(1024, 640);
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true;
        DoubleBuffered = true;

        var pitch = CreatePitchBounds(ClientRectangle);
        _world = new GameWorld(_input, pitch);

        _timer = new Timer { Interval = 16 };
        _timer.Tick += OnTick;
        _timer.Start();

        KeyDown += (_, e) => _input.OnKeyDown(e.KeyCode);
        KeyUp += (_, e) => _input.OnKeyUp(e.KeyCode);
        Resize += (_, _) => _world.ResizePitch(CreatePitchBounds(ClientRectangle));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        _renderer.Draw(e.Graphics, ClientRectangle, _world.Pitch, _world.HumanTeam, _world.CpuTeam, _world.Ball, _world.MatchState);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var ticks = _clock.ElapsedTicks;
        if (_lastTicks == 0)
        {
            _lastTicks = ticks;
            return;
        }

        var dt = (float)(ticks - _lastTicks) / Stopwatch.Frequency;
        _lastTicks = ticks;

        dt = Math.Clamp(dt, 0.001f, 0.033f);
        _world.Update(dt);

        Invalidate();
    }

    private static RectangleF CreatePitchBounds(Rectangle clientRect)
    {
        const float marginX = 40f;
        const float marginY = 60f;
        return new RectangleF(
            marginX,
            marginY,
            Math.Max(300f, clientRect.Width - marginX * 2f),
            Math.Max(220f, clientRect.Height - marginY * 2f));
    }
}
