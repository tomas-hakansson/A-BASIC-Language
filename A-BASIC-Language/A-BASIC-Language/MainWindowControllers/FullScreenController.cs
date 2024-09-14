namespace A_BASIC_Language.MainWindowControllers;

public class FullScreenController
{
    private static Rectangle _windowLocation;
    private readonly Form _owner;
    private readonly MenuStrip _menuStrip;
    private readonly ToolStrip _toolStrip;
    private readonly StatusStrip _statusStrip;

    public FullScreenController(Form owner, MenuStrip menuStrip, ToolStrip toolStrip, StatusStrip statusStrip)
    {
        _owner = owner;
        _menuStrip = menuStrip;
        _toolStrip = toolStrip;
        _statusStrip = statusStrip;
    }

    public void Set(bool fullScreen, Control terminalControl)
    {
        _menuStrip.Visible = !fullScreen;
        _toolStrip.Visible = !fullScreen;
        _statusStrip.Visible = !fullScreen;

        if (fullScreen)
        {
            _windowLocation = new Rectangle(_owner.Location.X, _owner.Location.Y, _owner.Width, _owner.Height);
            _owner.FormBorderStyle = FormBorderStyle.None;
            var s = Screen.FromRectangle(_windowLocation);
            _owner.Location = new Point(s.Bounds.X, s.Bounds.Y);
            _owner.Width = s.Bounds.Width;
            _owner.Height = s.Bounds.Height;
        }
        else
        {
            _owner.Location = new Point(_windowLocation.X, _windowLocation.Y);
            _owner.Width = _windowLocation.Width;
            _owner.Height = _windowLocation.Height;
            _owner.FormBorderStyle = FormBorderStyle.Sizable;
        }

        terminalControl.Focus();
    }
}