// See https://aka.ms/new-console-template for more information


namespace StageTwo
{
    internal class Line
    {
        public List<string> TextValues { get; set; }
        public List<Token> Tokens { get; set; }

        public Line()
        {
            TextValues = new List<string>();
            Tokens = new List<Token>();
        }

        public void Add(string textValue, Token token)
        {
            if (textValue != null && Enum.IsDefined<Token>(token))
            {
                TextValues.Add(textValue);
                Tokens.Add(token);
            }
        }
    }
}