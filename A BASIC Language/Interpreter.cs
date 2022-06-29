using System.Diagnostics;
using A_BASIC_Language.Gui;

namespace A_BASIC_Language;

internal class Interpreter
{
    /*
20 input n
25 i=sqr(sqr(n))
30 j=(i^2)^2
35 print n
38 print j
40 goto 20

[input n
i=sqr(sqr(n))
j=(i^2)^2
print n
print j
goto 20]

[20 -> 0
25 -> 1
30 -> 2
35 -> 3
38 -> 4
40 -> 5]

[input setvariable n
n sqr sqr setvariable i
i 2 ^ 2 ^ setvariable j
n print
j print
20 goto]
*/
    private Terminal? _terminal;
    readonly List<Line> _lines;//todo: replace with ParsedProgram.
    readonly Dictionary<int, int> _labelIndex;//todo: replace with ParsedProgram.
    readonly Dictionary<string, double?> _variables;//Ponder: do the value need to be nullable?
    readonly Stack<double> _data;

    int _index;

    public Interpreter(string pathToMain)
    {
        Parser parser = new(pathToMain);
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
            switch (line[i])
            {
                case Number n:
                    //put on stack.
                    _data.Push(n.Value);
                    break;
                case Variable v:
                {
                    //extract value.
                    //put on stack.
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
                    //pop value from stack
                    //set variable to value
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
                    //pop requisite number of arguments.
                    //evaluate.
                    //set stack if needed.
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
                            //get numeric value from console
                            //convert value to double
                            //push to data
                            var value = _terminal.ReadLine();
                            if (value != null && double.TryParse(value, out var numericValue))
                            {
                                _data.Push(numericValue);
                            }
                        }
                            break;
                        case "PRINT":
                        {
                            //pop value
                            //print
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
                            //pop label
                            //get new index from dict.
                            //set _index to new value
                            //decrement _index to account for loop incrementation.
                            //return
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