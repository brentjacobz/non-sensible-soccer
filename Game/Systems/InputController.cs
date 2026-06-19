using System.Numerics;

namespace NonSensibleSoccer.Game.Systems;

internal sealed class InputController
{
    private readonly HashSet<Keys> _keysDown = [];
    private bool _passConsumed;
    private bool _shootConsumed;
    private bool _switchConsumed;
    private bool _restartConsumed;
    private bool _difficultyBeginnerConsumed;
    private bool _difficultyNormalConsumed;

    public void OnKeyDown(Keys key) => _keysDown.Add(key);

    public void OnKeyUp(Keys key) => _keysDown.Remove(key);

    public Vector2 MovementDirection
    {
        get
        {
            var x = 0f;
            var y = 0f;

            if (_keysDown.Contains(Keys.A) || _keysDown.Contains(Keys.Left)) x -= 1f;
            if (_keysDown.Contains(Keys.D) || _keysDown.Contains(Keys.Right)) x += 1f;
            if (_keysDown.Contains(Keys.W) || _keysDown.Contains(Keys.Up)) y -= 1f;
            if (_keysDown.Contains(Keys.S) || _keysDown.Contains(Keys.Down)) y += 1f;

            var dir = new Vector2(x, y);
            return dir.LengthSquared() > 0 ? Vector2.Normalize(dir) : Vector2.Zero;
        }
    }

    public bool ConsumePassPressed()
    {
        var pressed = _keysDown.Contains(Keys.Space);
        if (pressed && !_passConsumed)
        {
            _passConsumed = true;
            return true;
        }

        if (!pressed)
        {
            _passConsumed = false;
        }

        return false;
    }

    public bool ConsumeShootPressed()
    {
        var pressed = _keysDown.Contains(Keys.LControlKey) || _keysDown.Contains(Keys.RControlKey) || _keysDown.Contains(Keys.ControlKey);
        if (pressed && !_shootConsumed)
        {
            _shootConsumed = true;
            return true;
        }

        if (!pressed)
        {
            _shootConsumed = false;
        }

        return false;
    }

    public bool ConsumeSwitchPressed()
    {
        var pressed = _keysDown.Contains(Keys.Tab) || _keysDown.Contains(Keys.Q);
        if (pressed && !_switchConsumed)
        {
            _switchConsumed = true;
            return true;
        }

        if (!pressed)
        {
            _switchConsumed = false;
        }

        return false;
    }

    public bool ConsumeRestartPressed()
    {
        var pressed = _keysDown.Contains(Keys.Enter);
        if (pressed && !_restartConsumed)
        {
            _restartConsumed = true;
            return true;
        }

        if (!pressed)
        {
            _restartConsumed = false;
        }

        return false;
    }

    public bool ConsumeBeginnerPressed()
    {
        var pressed = _keysDown.Contains(Keys.D1) || _keysDown.Contains(Keys.NumPad1) || _keysDown.Contains(Keys.F1);
        if (pressed && !_difficultyBeginnerConsumed)
        {
            _difficultyBeginnerConsumed = true;
            return true;
        }

        if (!pressed)
        {
            _difficultyBeginnerConsumed = false;
        }

        return false;
    }

    public bool ConsumeNormalPressed()
    {
        var pressed = _keysDown.Contains(Keys.D2) || _keysDown.Contains(Keys.NumPad2) || _keysDown.Contains(Keys.F2);
        if (pressed && !_difficultyNormalConsumed)
        {
            _difficultyNormalConsumed = true;
            return true;
        }

        if (!pressed)
        {
            _difficultyNormalConsumed = false;
        }

        return false;
    }
}
