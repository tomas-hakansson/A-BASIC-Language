namespace A_BASIC_Language.Gui.WinForms;

public class OverlayRenderer : IDisposable
{
    private Font ScaleIndependentFont { get; }

    public OverlayRenderer()
    {
        ScaleIndependentFont = new Font("Arial", 12.0f);
    }

    public void Render(Graphics g, bool active, bool cursorBlink, Rectangle clientRectangle)
    {
        if (active)
        {

        }
        else
        {
            const string inactiveMessage = "This window is not active.";
            using var blueBrush = new SolidBrush(Color.FromArgb(cursorBlink ? 100 : 170, 0, 0, 250));
            g.FillRectangle(blueBrush, 0, 18, clientRectangle.Width, ScaleIndependentFont.Height + 3);
            g.DrawString(inactiveMessage, ScaleIndependentFont, Brushes.White, 20, 20);
            g.DrawRectangle(Pens.White, -1, 18, clientRectangle.Width + 2, ScaleIndependentFont.Height + 3);
        }
    }

    public void Dispose()
    {
        ScaleIndependentFont.Dispose();
    }
}