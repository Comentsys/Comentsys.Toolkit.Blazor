using System.Drawing;

namespace Comentsys.Toolkit.Blazor.Demo.Pages;

public partial class Game
{
    private const int BoardSize = 7;
    private const int WinningScore = 100;
    private const int StartingSeconds = 60;
    private static readonly TimeSpan BeamDuration = TimeSpan.FromMilliseconds(450);
    private static readonly TimeSpan StickMoveDelay = TimeSpan.FromMilliseconds(140);

    private readonly System.Timers.Timer _timer = new(1000);
    private readonly Random _random = new();
    private readonly Color Comentsys = Color.FromArgb(132, 0, 132);
    private readonly Color ComentsysLight = Color.FromArgb(163, 64, 163);
    private readonly Color ComentsysDark = Color.FromArgb(90, 0, 92);
    private int _shipX = BoardSize / 2;
    private int _shipY = BoardSize / 2;
    private int _cometX = 1;
    private int _cometY = 1;
    private int _score;
    private int _remainingSeconds = StartingSeconds;
    private bool _isRunning = true;
    private DateTime _lastStickMove = DateTime.MinValue;
    private DateTime _beamUntil = DateTime.MinValue;
    private DirectionalPadDirection? _beamDirection;
    private string _lastAction = "Move diagonally or straight with the stick, line up the comet, then fire with the D-pad.";

    private Color[] PadFills => [Comentsys, ComentsysLight, ComentsysDark, ComentsysLight];
    private string ElapsedTime => $"{_remainingSeconds / 60:00}:{_remainingSeconds % 60:00}";
    private string ScoreDisplay => Math.Min(_score, 9999).ToString("0000");
    private string Status => _isRunning
        ? _lastAction
        : _score >= WinningScore
            ? $"Victory with {ScoreDisplay} points and {ElapsedTime} left."
            : $"Time up with {ScoreDisplay} points. Start a new game to play again.";

    protected override void OnInitialized()
    {
        PlaceComet();
        _timer.Elapsed += HandleTick;
        _timer.Start();
    }

    private void HandleTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (!_isRunning)
            return;

        _remainingSeconds--;
        if (_remainingSeconds <= 0)
        {
            _remainingSeconds = 0;
            _isRunning = false;
        }

        InvokeAsync(StateHasChanged);
    }

    private void StartGame()
    {
        _score = 0;
        _remainingSeconds = StartingSeconds;
        _isRunning = true;
        _beamDirection = null;
        _beamUntil = DateTime.MinValue;
        _lastAction = "Move diagonally or straight with the stick, line up the comet, then fire with the D-pad.";
        ResetShip();
        PlaceComet();
    }

    private void ResetShip()
    {
        _shipX = BoardSize / 2;
        _shipY = BoardSize / 2;
    }

    private void MoveWithStick(DirectionalStickValue value)
    {
        if (!_isRunning || value.Ratio < 0.25 || DateTime.UtcNow - _lastStickMove < StickMoveDelay)
            return;

        var radians = value.Angle * Math.PI / 180;
        var xAxis = Math.Cos(radians);
        var yAxis = Math.Sin(radians);
        var dx = Math.Abs(xAxis) >= 0.35 ? Math.Sign(xAxis) : 0;
        var dy = Math.Abs(yAxis) >= 0.35 ? Math.Sign(yAxis) : 0;

        _shipX = Math.Clamp(_shipX + dx, 0, BoardSize - 1);
        _shipY = Math.Clamp(_shipY + dy, 0, BoardSize - 1);
        _lastStickMove = DateTime.UtcNow;
        _lastAction = "Ship moved. Use the D-pad to fire when lined up with the comet.";
    }

    private void FireBeam(DirectionalPadDirection direction)
    {
        if (!_isRunning)
            return;

        _beamDirection = direction;
        _beamUntil = DateTime.UtcNow + BeamDuration;

        if (IsCometInBeam(direction))
        {
            _score += 10;
            _lastAction = "Direct hit. Move and line up the next comet.";

            if (_score >= WinningScore)
            {
                _isRunning = false;
                return;
            }

            PlaceComet();
        }
        else
        {
            _remainingSeconds = Math.Max(0, _remainingSeconds - 2);
            _lastAction = "Miss. The shot must share the comet's row or column.";
            if (_remainingSeconds == 0)
                _isRunning = false;
        }
    }

    private bool IsCometInBeam(DirectionalPadDirection direction) => direction switch
    {
        DirectionalPadDirection.Up => _cometX == _shipX && _cometY < _shipY,
        DirectionalPadDirection.Right => _cometY == _shipY && _cometX > _shipX,
        DirectionalPadDirection.Down => _cometX == _shipX && _cometY > _shipY,
        DirectionalPadDirection.Left => _cometY == _shipY && _cometX < _shipX,
        _ => false
    };

    private void PlaceComet()
    {
        do
        {
            _cometX = _random.Next(BoardSize);
            _cometY = _random.Next(BoardSize);
        }
        while (_cometX == _shipX && _cometY == _shipY);
    }

    private string GetCellClass(int x, int y)
    {
        var cell = "game-cell";
        if (x == _shipX && y == _shipY)
            cell += " player";
        else if (x == _cometX && y == _cometY)
            cell += " target";
        else if (IsBeamCell(x, y))
            cell += " beam";
        return cell;
    }

    private bool IsBeamCell(int x, int y)
    {
        if (_beamDirection is null || DateTime.UtcNow > _beamUntil)
            return false;

        return _beamDirection switch
        {
            DirectionalPadDirection.Up => x == _shipX && y < _shipY,
            DirectionalPadDirection.Right => y == _shipY && x > _shipX,
            DirectionalPadDirection.Down => x == _shipX && y > _shipY,
            DirectionalPadDirection.Left => y == _shipY && x < _shipX,
            _ => false
        };
    }

    public void Dispose()
    {
        _timer.Elapsed -= HandleTick;
        _timer.Dispose();
    }
}