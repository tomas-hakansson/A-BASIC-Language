namespace A_BASIC_Language.Gui.WinForms;

public class OverlayRenderer : IDisposable
{
    private readonly ImageList _imageList;
    private Font ScaleIndependentFont { get; }

    public OverlayRenderer(ImageList imageList)
    {
        _imageList = imageList;
        ScaleIndependentFont = new Font("Arial", 12.0f);
    }

    public void Render(Graphics g, bool active, bool cursorBlink, Rectangle clientRectangle, TerminalState state)
    {
        if (active)
        {
            switch (state)
            {
                case TerminalState.Empty:
                    g.DrawImageUnscaled(_imageList.Images[0], clientRectangle.Width - 18, 2);
                    break;
                case TerminalState.Running:
                    g.DrawImageUnscaled(cursorBlink ? _imageList.Images[1] : _imageList.Images[0], clientRectangle.Width - 18, 2);
                    break;
                case TerminalState.Ended:
                    g.DrawImageUnscaled(cursorBlink ? _imageList.Images[2] : _imageList.Images[0], clientRectangle.Width - 18, 2);
                    break;
            }
        }
        else
        {
            const string inactiveMessage = "This window is not active.";

            var size = g.MeasureString(inactiveMessage, ScaleIndependentFont);
            var x = (int) (clientRectangle.Width / 2.0 - size.Width / 2);
            using var blueBrush = new SolidBrush(Color.FromArgb(cursorBlink ? 250 : 170, 0, 0, 250));
            g.FillRectangle(blueBrush, 0, 18, clientRectangle.Width, ScaleIndependentFont.Height + 3);
            g.DrawString(inactiveMessage, ScaleIndependentFont, Brushes.White, x, 20);
            g.DrawRectangle(Pens.White, -1, 18, clientRectangle.Width + 2, ScaleIndependentFont.Height + 3);
        }
    }

    public void Dispose()
    {
        ScaleIndependentFont.Dispose();
    }
}