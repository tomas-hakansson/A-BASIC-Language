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

    public KeyboardController(Matrix characters, KeyDownOperationCompletedDelegate keyDownOperationCompleted, TerminalEmulatorStateStructure ts, Action toggleFullScreen, Action scrollUp, Action saveLineInput, Action saveDirectModeInput, Action moveLineInputLeft) {
        _characters = characters;
        _keyDownOperationCompleted = keyDownOperationCompleted;
        _ts = ts;
        _toggleFullScreen = toggleFullScreen;
        _scrollUp = scrollUp;
        _saveLineInput = saveLineInput;
        _saveDirectModeInput = saveDirectModeInput;
        _moveLineInputLeft = moveLineInputLeft;
    }

    public void HandleKeyDown(KeyEventArgs e, TerminalEmulatorStateStructure ts)
    {
        switch (e.KeyCode)
        {
            case Keys.F11:
                _toggleFullScreen();
                break;
            case Keys.Enter:
                if (_ts.LineInputMode)
                    _saveLineInput();
                else if (_ts.State == TerminalState.Empty || _ts.State == TerminalState.Ended)
                    _saveDirectModeInput();

                _ts.CursorY++;

                if (_ts.CursorY >= _characters.RowCount)
                {
                    _ts.CursorY = _characters.RowCount - 1;
                    _scrollUp();
                }

                _ts.CursorX = 0;

                _keyDownOperationCompleted(ref e);

                if (_ts.LineInputMode)
                    _ts.LineInputMode = false;

                break;
            case Keys.Up:
                _ts.CursorY--;

                if (_ts.CursorY < 0)
                    _ts.CursorY = 0;

                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Down:
                _ts.CursorY++;

                if (_ts.CursorY >= _characters.RowCount)
                {
                    _ts.CursorY = _characters.RowCount - 1;
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
                _ts.CursorY = 0;
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.PageDown:
                if (_ts.CursorY == _characters.RowCount - 1)
                    _scrollUp();
                else
                    _ts.CursorY = _characters.RowCount - 1;
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Insert:
                if (_ts.CursorX == _characters.ColumnCount - 1 && _ts.CursorY == _characters.RowCount - 1)
                {
                    _characters.SetAt(_ts.CursorX, _ts.CursorY, ' ');
                    return;
                }
                _characters.InsertAt(_ts.CursorX, _ts.CursorY);
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Delete:
                if (_ts.CursorX == _characters.ColumnCount - 1 && _ts.CursorY == _characters.RowCount - 1)
                {
                    _characters.SetAt(_ts.CursorX, _ts.CursorY, ' ');
                    return;
                }
                _characters.DeleteAt(_ts.CursorX, _ts.CursorY);
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Home:
                while (_ts.CursorX > 0)
                    _ts.CursorLeft();
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.End:
                while (_ts.CursorX < _characters.ColumnCount - 1)
                    _ts.CursorRight();
                _keyDownOperationCompleted(ref e);
                break;
            case Keys.Back:
                if (_ts.CursorX <= 0 && _ts.CursorY <= 0)
                    return;

                if (_ts.LineInputMode)
                    if (_ts.LineInputY > _ts.CursorY || (_ts.LineInputY == _ts.CursorY && _ts.LineInputX >= _ts.CursorX))
                        _moveLineInputLeft();

                _ts.CursorLeft();
                _characters.DeleteAt(_ts.CursorX, _ts.CursorY);
                _keyDownOperationCompleted(ref e);
                break;
        }
    }
}