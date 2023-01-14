using CharacterMatrix;

namespace A_BASIC_Language.Gui.WinForms;

public class KeyboardController
{
    public delegate void KeyDownOperationCompletedDelegate(ref KeyEventArgs eventArgs);

    private readonly Matrix _characters;
    private readonly KeyDownOperationCompletedDelegate _keyDownOperationCompleted;
    private readonly TerminalEmulatorStateStructure _ts;
    private readonly Action _toggleFullScreen;
    private readonly Action _scrollUp;
    private readonly Action _saveLineInput;
    private readonly Action _saveDirectModeInput;
    private readonly Action _moveLineInputLeft;
    private readonly Action _end;

    public KeyboardController(Matrix characters, KeyDownOperationCompletedDelegate keyDownOperationCompleted, TerminalEmulatorStateStructure ts, Action toggleFullScreen, Action scrollUp, Action saveLineInput, Action saveDirectModeInput, Action moveLineInputLeft, Action end)
    {
        _characters = characters;
        _keyDownOperationCompleted = keyDownOperationCompleted;
        _ts = ts;
        _toggleFullScreen = toggleFullScreen;
        _scrollUp = scrollUp;
        _saveLineInput = saveLineInput;
        _saveDirectModeInput = saveDirectModeInput;
        _moveLineInputLeft = moveLineInputLeft;
        _end = end;
    }

    public void HandleKeyDown(KeyEventArgs e, TerminalEmulatorStateStructure ts)
    {
        switch (e.KeyCode)
        {
            case Keys.F11:
                _toggleFullScreen();
                break;
            case Keys.C:
                if (e.Control)
                {
                    _ts.LineInputMode = false;
                    _ts.State = TerminalState.Ended;
                    _ts.UserBreak = true;
                    _end();
                }
                break;
            case Keys.Enter:
                if (_ts.LineInputMode)
                    _saveLineInput();
                else if (_ts.State == TerminalState.Empty || _ts.State == TerminalState.Ended)
                    _saveDirectModeInput();

                _ts.CursorPosition.Y++;

                if (_ts.CursorPosition.Y >= _characters.RowCount)
                {
                    _ts.CursorPosition.Y = _characters.RowCount - 1;
                    _scrollUp();
                }

                _ts.CursorPosition.X = 0;

                _keyDownOperationCompleted(ref e);

                if (_ts.LineInputMode)
                    _ts.LineInputMode = false;

                break;
            case Keys.Up:
                _ts.CursorPosition.Y--;

                if (_ts.CursorPosition.Y < 0)
                    _ts.CursorPosition.Y = 0;

                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Down:
                _ts.CursorPosition.Y++;

                if (_ts.CursorPosition.Y >= _characters.RowCount)
                {
                    _ts.CursorPosition.Y = _characters.RowCount - 1;
                    _scrollUp();
                }

                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Left:
                _ts.CursorLeft();
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Right:
                _ts.CursorRight();
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.PageUp:
                _ts.CursorPosition.Y = 0;
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.PageDown:
                if (_ts.CursorPosition.Y == _characters.RowCount - 1)
                    _scrollUp();
                else
                    _ts.CursorPosition.Y = _characters.RowCount - 1;
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Insert:
                if (_ts.CursorPosition.X == _characters.ColumnCount - 1 && _ts.CursorPosition.Y == _characters.RowCount - 1)
                {
                    _characters.SetAt(_ts.CursorPosition.X, _ts.CursorPosition.Y, ' ');
                    return;
                }
                _characters.InsertAt(_ts.CursorPosition.X, _ts.CursorPosition.Y);
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Delete:
                if (_ts.CursorPosition.X == _characters.ColumnCount - 1 && _ts.CursorPosition.Y == _characters.RowCount - 1)
                {
                    _characters.SetAt(_ts.CursorPosition.X, _ts.CursorPosition.Y, ' ');
                    return;
                }
                _characters.DeleteAt(_ts.CursorPosition.X, _ts.CursorPosition.Y);
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Home:
                while (_ts.CursorPosition.X > 0)
                    _ts.CursorLeft();
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.End:
                while (_ts.CursorPosition.X < _characters.ColumnCount - 1)
                    _ts.CursorRight();
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Back:
                if (_ts.CursorPosition is { X: <= 0, Y: <= 0 })
                    return;

                if (_ts.LineInputMode)
                    if (_ts.LineInputY > _ts.CursorPosition.Y || (_ts.LineInputY == _ts.CursorPosition.Y && _ts.LineInputX >= _ts.CursorPosition.X))
                        _moveLineInputLeft();

                _ts.CursorLeft();
                _characters.DeleteAt(_ts.CursorPosition.X, _ts.CursorPosition.Y);
                _keyDownOperationCompleted(ref e);
                break;
        }
    }
}