using System.Windows.Forms;

namespace A_BASIC_Language.Gui;

public static class MsgBox
{
    public static bool Ask(Form owner, string prompt) =>
        MessageBox.Show(owner, prompt, owner.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;

    public static void Fail(Form owner, string prompt) =>
        MessageBox.Show(owner, prompt, owner.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void Fail(Form owner, string prompt, string text) =>
        MessageBox.Show(owner, prompt, text, MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void Tell(Form owner, string prompt) =>
        MessageBox.Show(owner, prompt, owner.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void Tell(Form owner, string prompt, string text) =>
        MessageBox.Show(owner, prompt, text, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
