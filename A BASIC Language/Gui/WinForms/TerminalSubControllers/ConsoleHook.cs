using ConsoleControlLibrary;

namespace A_BASIC_Language.Gui.WinForms.TerminalSubControllers;

public class ConsoleHook
{
    private readonly Form _parentForm;
    private readonly ConsoleControl _c;
    private readonly System.Windows.Forms.Timer _restoreTimer;
    private int _restoreFormState;

    public ConsoleHook(ConsoleControl consoleControl, Form parentForm, System.Windows.Forms.Timer restoreTimer)
    {
        _c = consoleControl;
        _parentForm = parentForm;
        _restoreTimer = restoreTimer;
    }

    public void Hook(nint targetHandle, Control.ControlCollection targetCollection, TerminalEmulatorStateStructure terminalState)
    {
        _c.ColumnCount = terminalState.ColumnCount;
        _c.RowCount = terminalState.RowCount;
        _c.Dock = DockStyle.Fill;
        _c.State.CurrentForm = new LoadForm(targetHandle, _c);
        _c.Visible = false;
        _c.Enabled = false;
        _c.KeyDown += ConsoleForm_KeyDown;
        targetCollection.Add(_c);
        _restoreTimer.Tick += restoreTimer_Tick;
    }

    public void ShowSourceForm(string sourceCode, string programFilename)
    {
        _c.State.CurrentForm = new ViewSourceForm(_c.Handle, _c, sourceCode, programFilename);
        _c.SetDefaultColorScheme(new ControlColorScheme());
        _c.Visible = true;
        _c.Enabled = true;
        _c.Focus();
        ((ViewSourceForm)_c.State.CurrentForm).SetFocusToList();
    }

    private void ConsoleForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!_c.Visible)
            return;

        if (_c.State.CurrentForm is ViewSourceForm && e.KeyCode == Keys.Escape && _restoreTimer.Enabled == false)
            _restoreTimer.Enabled = true;
    }

    private void restoreTimer_Tick(object? sender, EventArgs e)
    {
        switch (_restoreFormState)
        {
            case 0:
                _restoreFormState++;
                break;
            case 1:
                _c.Visible = false;
                _c.Enabled = false;
                _restoreFormState++;
                break;
            default:
                _restoreTimer.Enabled = false;
                _restoreFormState = 0;
                _parentForm.Focus();
                break;
        }
    }
}