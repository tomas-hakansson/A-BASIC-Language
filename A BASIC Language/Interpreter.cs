using System.Diagnostics;
using A_BASIC_Language.Gui;

namespace A_BASIC_Language;

internal class Interpreter
{
    private Terminal? _terminal;
    readonly List<Line> _lines;//todo: replace with ParsedProgram.
    readonly Dictionary<int, int> _labelIndex;//todo: replace with ParsedProgram.
    readonly Dictionary<string, double?> _variables;//Ponder: do the value need to be nullable?
    readonly Stack<double> _data;

    int _index;

    public Interpreter(string source)
    {
        //10 PRINT "HEJ":PRINT "OJ"
        //"hej" print "nl" print "oj" print "nl" print
        //    new Line(new List<Token>() { new String("hej"), new Procedure("print"), new String("\n"), new Procedure("print"), new Assignment("N") }),...
        // 11 print a + b
        // a b + print
        // a + (4 * 3) / sqrt(b)
        //4 3 * b sqrt / 4 +
        //a + (b * ((c - d) ^ 4))
        // c d - 4 ^ b * a +
        // c d 4 b a - ^ * +
        Parser parser = new(source);
        var program = parser.Program;
        _lines = program.Lines;
        _labelIndex = program.LabelIndex;

        //_lines = new List<Line>
        //{
        //    new Line(new List<Token>() { new Procedure("INPUT"), new Assignment("N") }),
        //    new Line(new List<Token>() { new Variable("N"), new Procedure("SQR"), new Procedure("SQR"), new Assignment("I") }),
        //    new Line(new List<Token>() { new Variable("I"), new Number(2), new Procedure("^"), 
        //                                                    new Number(2), new Procedure("^"), new Assignment("J") }),
        //    new Line(new List<Token>() { new Variable("N"), new Procedure("PRINT") }),
        //    new Line(new List<Token>() { new Variable("J"), new Procedure("PRINT") }),
        //    new Line(new List<Token>() { new Number(20), new Procedure("GOTO") }),
        //};
        //_labelIndex = new Dictionary<int, int>
        //{
        //    { 20, 0 },
        //    { 25, 1 },
        //    { 30, 2 },
        //    { 35, 3 },
        //    { 38, 4 },
        //    { 40, 5 },
        //};
        _variables = new Dictionary<string, double?>();
        _data = new Stack<double>();

        _index = 0;
    }

    public void Run(Terminal terminal)
    {
        _terminal = terminal;

        for (; _index < _lines.Count; _index++)
        {
            Application.DoEvents();
            
            if (!_terminal.Running)
                return;

            EvalLine();
        }
    }

    private void EvalLine()
    {
        if (_terminal == null)
            throw new SystemException("Terminal not initialized.");

        Application.DoEvents();

        var line = _lines[_index];
        Debug.WriteLine($"Eval {_index}: {line}");

        for (int i = 0; i < line.Count; i++)
        {
            if (!_terminal.Running)
                return;

            switch (line[i])
            {
                case Number n:
                    _data.Push(n.Value);
                    break;
                case Variable v:
                {
                    if (_variables.TryGetValue(v.Symbol, out var value) && value.HasValue)
                    {
                        _data.Push(value.Value);
                    }
                    else
                    {
                        Debug.Fail("Something was wrong with the value");//fixme: really bad text.
                        //todo: error handling.
                    }
                }
                    break;
                case Assignment a:
                {
                    if (_data.Count > 0)
                    {
                        var value = _data.Pop();
                        var symbol = a.Symbol;
                        _variables[symbol] = value;
                    }
                    else
                        Debug.Fail("The stack is empty");
                }
                    break;
                case Procedure p:
                    switch (p.Name)
                    {
                        case "^":
                        {
                            if (_data.Count >= 2)
                            {
                                var x = _data.Pop();
                                var y = _data.Pop();
                                var result = Math.Pow(y, x);
                                _data.Push(result);
                            }
                            else
                                Debug.Fail("Insufficient items on the stack");
                        }
                            break;
                        case "SQR":
                        {
                            if (_data.Count > 0)
                            {
                                var x = _data.Pop();
                                var result = Math.Sqrt(x);
                                _data.Push(result);
                            }
                            else
                                Debug.Fail("The stack is empty");
                        }
                            break;
                        case "INPUT":
                        {
                            var value = _terminal.ReadLine();
                            if (value != null && double.TryParse(value, out var numericValue))
                            {
                                _data.Push(numericValue);
                            }
                        }
                            break;
                        case "PRINT":
                        {
                            if (_data.Count > 0)
                            {
                                var value = _data.Pop();
                                _terminal.WriteLine(value.ToString());
                            }
                            else
                                Debug.Fail("The stack is empty");
                        }
                            break;
                        case "GOTO":
                        {
                            if (_data.Count > 0)
                            {
                                var label = _data.Pop();
                                if (_labelIndex.TryGetValue((int)label, out var newIndex))
                                {
                                    _index = (int)newIndex;
                                    _index--;//HACK: come up with something better.
                                    return;
                                }
                                else
                                {
                                    Debug.Fail("Something was wrong with the value");//fixme; really bad text.
                                    //todo: error handling.
                                    throw new InvalidOperationException("this is only to get the c# to stop complaining.");
                                }
                            }
                            else
                                Debug.Fail("The stack is empty");
                        }
                            break;
                        default:
                            //todo :error handling.
                            break;
                    }
                    break;
                default:
                    //todo: error handling or remove this.
                    break;
            }
        }
    }
}