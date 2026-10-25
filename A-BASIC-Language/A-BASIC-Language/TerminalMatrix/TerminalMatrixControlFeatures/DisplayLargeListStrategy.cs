using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using TerminalMatrixNetFramework;

namespace A_BASIC_Language.TerminalMatrix.TerminalMatrixControlFeatures;

public static class DisplayLargeListStrategy
{
    public static void List(Action<string> writeLine, Action refresh, SortedDictionary<int, ProgramLine> programLines)
    {
        var lines = new List<ProgramLine>(programLines.Values);

        if (lines.Count > 1000)
        {
            for (var i = 0; i < 200; i++)
            {
                writeLine(lines[i].RawString);
            }

            for (var i = 200; i < 400; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Application.DoEvents();
            }

            for (var i = 400; i < 600; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(1);
                Application.DoEvents();
            }

            for (var i = 600; i < 990; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(2);
                Application.DoEvents();
            }

            for (var i = 990; i < lines.Count; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
        else if (lines.Count > 800)
        {
            for (var i = 0; i < 180; i++)
            {
                writeLine(lines[i].RawString);
                Application.DoEvents();
            }

            for (var i = 180; i < 360; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Application.DoEvents();
            }

            for (var i = 360; i < 540; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(1);
                Application.DoEvents();
            }

            for (var i = 540; i < 790; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(2);
                Application.DoEvents();
            }

            for (var i = 790; i < lines.Count; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
        else if (lines.Count > 600)
        {
            for (var i = 0; i < 200; i++)
            {
                writeLine(lines[i].RawString);
                Application.DoEvents();
            }

            for (var i = 200; i < 300; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Application.DoEvents();
            }

            for (var i = 300; i < 400; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(1);
                Application.DoEvents();
            }

            for (var i = 400; i < lines.Count; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
        else if (lines.Count > 400)
        {
            for (var i = 0; i < 100; i++)
            {
                writeLine(lines[i].RawString);
                Application.DoEvents();
            }

            for (var i = 100; i < 200; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Application.DoEvents();
            }

            for (var i = 200; i < 300; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(1);
                Application.DoEvents();
            }

            for (var i = 300; i < lines.Count; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
        else if (lines.Count > 200)
        {
            for (var i = 0; i < 50; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(1);
                Application.DoEvents();
            }

            for (var i = 50; i < 150; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(2);
                Application.DoEvents();
            }

            for (var i = 150; i < lines.Count; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
        else if (lines.Count > 100)
        {
            for (var i = 0; i < 70; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(2);
                Application.DoEvents();
            }

            for (var i = 70; i < lines.Count; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
        else if (lines.Count > 60)
        {
            for (var i = 0; i < 40; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(2);
                Application.DoEvents();
            }

            for (var i = 40; i < lines.Count; i++)
            {
                writeLine(lines[i].RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
        else
        {
            foreach (var programLine in lines)
            {
                writeLine(programLine.RawString);
                refresh();
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }
    }
}
