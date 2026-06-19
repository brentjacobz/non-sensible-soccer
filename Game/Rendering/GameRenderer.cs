using NonSensibleSoccer.Game.Models;

namespace NonSensibleSoccer.Game.Rendering;

internal sealed class GameRenderer
{
    private readonly Font _hudFont = new("Segoe UI", 13f, FontStyle.Bold, GraphicsUnit.Pixel);
    private readonly Font _messageFont = new("Segoe UI", 16f, FontStyle.Bold, GraphicsUnit.Pixel);

    public void Draw(Graphics g, Rectangle clientRect, RectangleF pitch, Team human, Team cpu, Ball ball, MatchState state)
    {
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.FromArgb(14, 95, 37));

        DrawPitch(g, pitch);
        DrawTeam(g, cpu, Color.FromArgb(232, 70, 70), Color.FromArgb(255, 245, 230));
        DrawTeam(g, human, Color.FromArgb(61, 168, 250), Color.FromArgb(255, 245, 230));
        DrawBall(g, ball);
        DrawOwnerMarker(g, ball);
        DrawHud(g, clientRect, human, cpu, ball, state);
    }

    private static void DrawPitch(Graphics g, RectangleF pitch)
    {
        using var linePen = new Pen(Color.FromArgb(240, 250, 240), 2f);
        using var grassBrush = new SolidBrush(Color.FromArgb(20, 122, 49));

        g.FillRectangle(grassBrush, pitch);
        g.DrawRectangle(linePen, pitch.X, pitch.Y, pitch.Width, pitch.Height);

        var center = new PointF(pitch.Left + pitch.Width * 0.5f, pitch.Top + pitch.Height * 0.5f);
        g.DrawLine(linePen, center.X, pitch.Top, center.X, pitch.Bottom);
        g.DrawEllipse(linePen, center.X - 55f, center.Y - 55f, 110f, 110f);

        var goalHeight = pitch.Height * 0.3f;
        var goalTop = pitch.Top + (pitch.Height - goalHeight) * 0.5f;
        g.DrawRectangle(linePen, pitch.Left - 12f, goalTop, 12f, goalHeight);
        g.DrawRectangle(linePen, pitch.Right, goalTop, 12f, goalHeight);
    }

    private static void DrawTeam(Graphics g, Team team, Color shirtColor, Color activeOutline)
    {
        using var shirtBrush = new SolidBrush(shirtColor);
        using var activePen = new Pen(activeOutline, 2.8f);

        foreach (var player in team.Players)
        {
            var diameter = player.Radius * 2f;
            var x = player.Position.X - player.Radius;
            var y = player.Position.Y - player.Radius;
            g.FillEllipse(shirtBrush, x, y, diameter, diameter);

            if (player.IsActive && team.IsHuman)
            {
                g.DrawEllipse(activePen, x - 2f, y - 2f, diameter + 4f, diameter + 4f);
            }
        }
    }

    private static void DrawBall(Graphics g, Ball ball)
    {
        using var ballBrush = new SolidBrush(Color.WhiteSmoke);
        using var ballPen = new Pen(Color.Black, 1.2f);
        var d = ball.Radius * 2f;
        g.FillEllipse(ballBrush, ball.Position.X - ball.Radius, ball.Position.Y - ball.Radius, d, d);
        g.DrawEllipse(ballPen, ball.Position.X - ball.Radius, ball.Position.Y - ball.Radius, d, d);
    }

    private static void DrawOwnerMarker(Graphics g, Ball ball)
    {
        if (ball.Owner is null)
        {
            return;
        }

        var owner = ball.Owner;
        var markerY = owner.Position.Y - owner.Radius - 16f;
        var markerX = owner.Position.X;
        var markerColor = owner.IsHumanTeam ? Color.FromArgb(255, 250, 221, 97) : Color.FromArgb(255, 255, 159, 122);

        using var markerBrush = new SolidBrush(markerColor);
        using var markerPen = new Pen(Color.FromArgb(200, 35, 35, 35), 1.2f);

        var points = new[]
        {
            new PointF(markerX, markerY),
            new PointF(markerX - 6f, markerY - 10f),
            new PointF(markerX + 6f, markerY - 10f)
        };

        g.FillPolygon(markerBrush, points);
        g.DrawPolygon(markerPen, points);
    }

    private void DrawHud(Graphics g, Rectangle clientRect, Team human, Team cpu, Ball ball, MatchState state)
    {
        var scoreText = $"YOU {human.Score} - {cpu.Score} CPU";
        using var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0));
        using var textBrush = new SolidBrush(Color.WhiteSmoke);
        g.DrawString(scoreText, _hudFont, shadowBrush, 18f, 16f);
        g.DrawString(scoreText, _hudFont, textBrush, 16f, 14f);

        var controlsText = "WASD/Arrows Move | Q/Tab Switch | Space Pass | Ctrl Shoot | 1/F1 Beginner | 2/F2 Normal";
        g.DrawString(controlsText, _hudFont, textBrush, 16f, clientRect.Height - 30f);

        var possessionText = "Possession: Loose Ball";
        if (ball.Owner is not null)
        {
            possessionText = ball.Owner.IsHumanTeam ? "Possession: You" : "Possession: CPU";
        }

        g.DrawString(possessionText, _hudFont, textBrush, clientRect.Width - 190f, 14f);

        var difficultyText = $"Difficulty: {state.Difficulty}";
        g.DrawString(difficultyText, _hudFont, textBrush, 16f, 34f);

        if (!string.IsNullOrWhiteSpace(state.Message))
        {
            var size = g.MeasureString(state.Message, _messageFont);
            var x = (clientRect.Width - size.Width) * 0.5f;
            var y = 18f;
            g.DrawString(state.Message, _messageFont, shadowBrush, x + 2f, y + 2f);
            g.DrawString(state.Message, _messageFont, textBrush, x, y);
        }
    }
}
