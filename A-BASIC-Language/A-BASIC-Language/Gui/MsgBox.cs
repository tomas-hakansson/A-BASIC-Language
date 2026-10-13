using System.Windows.Forms;

namespace A_BASIC_Language.Gui;

public interface IMsgBox
{
    bool Ask(string prompt);
    void Fail(string prompt);
    void Fail(string prompt, string text);
    void Tell(string prompt);
    void Tell(string prompt, string text);
}

public class MsgBox : IMsgBox
{
    private readonly Form _owner;

    public MsgBox(Form owner)
    {
        _owner = owner;
    }

    public bool Ask(string prompt) =>
        MessageBox.Show(_owner, prompt, _owner.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;

    public void Fail(string prompt) =>
        MessageBox.Show(_owner, prompt, _owner.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public void Fail(string prompt, string text) =>
        MessageBox.Show(_owner, prompt, text, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public void Tell(string prompt) =>
        MessageBox.Show(_owner, prompt, _owner.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

    public void Tell(string prompt, string text) =>
        MessageBox.Show(_owner, prompt, text, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
