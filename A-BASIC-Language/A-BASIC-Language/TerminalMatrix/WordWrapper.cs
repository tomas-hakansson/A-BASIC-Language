using System;
using System.Collections.Generic;
using System.Text;

namespace TerminalMatrixNetFramework;

internal static class WordWrapper
{
    public static List<string> WordWrap(string text, int width)
    {
        var result = new List<string>();

        static int Break(string breakText, int breakPos, int max)
        {
            var position = max;

            while (position >= 0 && !char.IsWhiteSpace(breakText[breakPos + position]))
                position--;

            if (position < 0)
                return max;

            while (position >= 0 && char.IsWhiteSpace(breakText[breakPos + position]))
                position--;

            return position + 1;
        }

        int wordBreak;
        var s = new StringBuilder();

        for (var charPointer = 0; charPointer < text.Length; charPointer = wordBreak)
        {
            var endOfLine = text.IndexOf("\r\n", charPointer, StringComparison.Ordinal);

            if (endOfLine < 0)
                endOfLine = text.Length;

            wordBreak = endOfLine <= 0 ? text.Length : endOfLine + 2;

            if (endOfLine > charPointer)
            {
                do
                {
                    var length = endOfLine - charPointer;
                    
                    if (length > width)
                        length = Break(text, charPointer, width);
                    
                    s.Append(text, charPointer, length);
                    result.Add(s.ToString());
                    s.Clear();
                    charPointer += length;
                    
                    while (charPointer < endOfLine && char.IsWhiteSpace(text[charPointer]))
                        charPointer++;

                } while (endOfLine > charPointer);

                continue;
            }

            result.Add(s.ToString());
            s.Clear();
        }

        return result;
    }
}
