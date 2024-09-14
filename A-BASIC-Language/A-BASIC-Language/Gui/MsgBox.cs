namespace A_BASIC_Language.Gui;

public static class MsgBox
{
    public static bool Ask(Form owner, string prompt) =>
        MessageBox.Show(owner, prompt, owner.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
}