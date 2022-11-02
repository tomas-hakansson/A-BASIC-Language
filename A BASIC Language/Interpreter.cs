using System.Diagnostics;
using A_BASIC_Language.Gui;
using A_BASIC_Language.SpecificExecutors;
using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language;

public class Interpreter
{
    private readonly bool _empty;
    const string TheProgramHasEnded = "The program has ended";
    Terminal? _terminal;
    readonly Stage2.ParseResult _parseResult;
    readonly Dictionary<string, ValueBase?> _variables;//Ponder: do the value need to be nullable?
    readonly Dictionary<string, Dimension> _dimVariables;
    readonly Stack<ValueBase> _data;
    bool EndMessageDisplayed { get; set; }

    readonly Random _random;//Note: For the RND function.
    int _currentLineNumber;

    public Interpreter(string source)
    {
        _empty = string.IsNullOrWhiteSpace(source);
        Parser parser = new(source);
        _parseResult = parser.Result;
        _variables = new Dictionary<string, ValueBase?>();
        _dimVariables = new Dictionary<string, Dimension>();
        _data = new Stack<ValueBase>();
        _random = new Random();
        _currentLineNumber = 0;
    }

    public void Run(Terminal terminal)
    {
        _terminal = terminal;

        if (_terminal.State != TerminalState.Running)
            return;

        Eval();
    }

    void Eval()
    {
        EndMessageDisplayed = false;

        if (_terminal == null)
            throw new SystemException("Terminal not initialized.");

        Application.DoEvents();

        var addExecutor = new AddExecutor(_data);
        var subtractExecutor = new SubtractExecutor(_data);
        var comparisonExecutor = new ComparisonExecutor(_data);
        var flatVariableExecutor = new FlatVariableExecutor(_data,End, _variables);
        var dimExecutor = new DimExecutor(_data, End, _dimVariables);

        Application.DoEvents();

        var endProgram = false;

        for (int i = 0; i < _parseResult.EvalValues.Count; i++)
        {
            if (endProgram)
                return;
            if (_terminal.State != TerminalState.Running)
                return;

            switch (_parseResult.EvalValues[i])
            {
                case ABL_Label lbl:
                    _currentLineNumber = lbl.Value;
                    //Note: NOP.
                    break;
                case ABL_Number n:
                    _data.Push(ValueBase.GetValueType(n.Value));
                    break;
                case ABL_String s:
                    _data.Push(new StringValue(s.Value));
                    break;
                case ABL_DIM_Creation dc:
                    dimExecutor.Create(dc);
                    break;
                case ABL_Variable v:
                    flatVariableExecutor.Create(v);
                    break;
                case ABL_DIM_Variable dv:
                    dimExecutor.Read(dv);
                    break;
                case ABL_Assignment a:
                    flatVariableExecutor.Write(a);
                    break;
                case ABL_DIM_Assignment da:
                    dimExecutor.Write(da); //Note: E.g. 10 dimvariable(1, 2) = 42.
                    break;
                case ABL_Procedure p:
                    switch (p.Name)
                    {
                        case "^":
                            {
                                if (_data.Count >= 2)
                                {
                                    var x = _data.Pop();
                                    var y = _data.Pop();

                                    //TODO: Type checking
                                    var result = Math.Pow((double)y.GetValueAsType<FloatValue>(), (double)x.GetValueAsType<FloatValue>());
                                    _data.Push(new FloatValue(result));
                                }
                                else
                                    Debug.Fail("Insufficient items on the stack");
                            }
                            break;
                        case "*":
                            {
                                if (_data.Count >= 2)
                                {
                                    var x = _data.Pop();
                                    var y = _data.Pop();

                                    //TODO: Type checking
                                    var result = (double)y.GetValueAsType<FloatValue>() * (double)x.GetValueAsType<FloatValue>();
                                    _data.Push(new FloatValue(result));
                                }
                                else
                                    Debug.Fail("Insufficient items on the stack");
                            }
                            break;
                        case "+":
                            addExecutor.Run(_currentLineNumber);
                            break;
                        case "-":
                            subtractExecutor.Run(_currentLineNumber);
                            break;
                        case ">":
                            comparisonExecutor.Run(_currentLineNumber, (a, b) => a > b);
                            break;
                        case ">=":
                            comparisonExecutor.Run(_currentLineNumber, (a, b) => a >= b);
                            break;
                        case "<":
                            comparisonExecutor.Run(_currentLineNumber, (a, b) => a < b);
                            break;
                        case "<=":
                            comparisonExecutor.Run(_currentLineNumber, (a, b) => a <= b);
                            break;
                        case "=":
                            comparisonExecutor.Run(_currentLineNumber, (a, b) => Math.Abs(a - b) < 0.00001);
                            break;
                        case "!=":
                        case "<>":
                            comparisonExecutor.Run(_currentLineNumber, (a, b) => Math.Abs(a - b) > 0.00001);
                            break;
                        case "#END-PROGRAM":
                            endProgram = true;
                            break;
                        case "GOTO":
                            {
                                if (_data.Count > 0)
                                {
                                    var label = _data.Pop();
                                    //TODO: Type checking
                                    if (_parseResult.LabelIndex.TryGetValue((int)label.GetValueAsType<IntValue>(), out var newIndex))
                                    {
                                        i = newIndex;
                                        i--;//HACK: come up with something better.
                                    }
                                    else
                                    {
                                        Debug.Fail("Something was wrong with the value");//fixme; really bad text.
                                                                                         ////todo: error handling.
                                        throw new InvalidOperationException("this is only to get c# to stop complaining.");
                                    }
                                }
                                else
                                    Debug.Fail("The stack is empty");
                            }
                            break;
                        case "#IF-FALSE-GOTO":
                            {
                                if (_data.Count >= 2)
                                {
                                    var label = _data.Pop();
                                    var boolean = _data.Pop();
                                    //TODO: Type checking
                                    if ((double)boolean.GetValueAsType<FloatValue>() == 0)//Note: if it's false.
                                    {
                                        if (_parseResult.LabelIndex.TryGetValue((int)label.GetValueAsType<IntValue>(), out var newIndex))
                                        {
                                            i = newIndex;
                                            i--;//HACK: come up with something better.
                                        }
                                        else
                                        {
                                            Debug.Fail("Something was wrong with the value");//fixme; really bad text.
                                                                                             //todo: error handling.
                                            throw new InvalidOperationException("this is only to get c# to stop complaining.");
                                        }
                                    }
                                }
                                else
                                    Debug.Fail("The stack is empty");
                            }
                            break;
                        case "#INPUT-FLOAT":
                            {
                                var value = ValueBase.GetValueType(_terminal.ReadLine()); // TODO: Must support "redo from start".
                                _data.Push(value);
                            }
                            break;
                        case "#INPUT-STRING":
                            {
                                var value = ValueBase.GetValueType(_terminal.ReadLine()); // TODO: Must support "redo from start".
                                _data.Push(value);
                            }
                            break;
                        case "INT":
                            {
                                var value = _data.Pop();
                                var result = (int)value.GetValueAsType<IntValue>();
                                _data.Push(new IntValue(result));
                            }
                            break;
                        case "#NEXT-LINE":
                            _terminal.WriteLine();
                            break;
                        case "#NEXT-TAB-POSITION":
                            _terminal.NextTab();
                            break;
                        case "RND":
                            //ToDo: implement this properly.
                            {
                                if (_data.Count > 0)
                                {
                                    _ = _data.Pop();//Note: We are currently not using this value but probably will later.
                                    var result = _random.NextDouble();
                                    _data.Push(new FloatValue(result));
                                }
                                else
                                    Debug.Fail("The stack is empty");
                            }
                            break;
                        case "SQR":
                            {
                                if (_data.Count > 0)
                                {
                                    var x = _data.Pop();
                                    //TODO: Type checking
                                    var result = Math.Sqrt((double)x.GetValueAsType<FloatValue>());
                                    _data.Push(new FloatValue(result));
                                }
                                else
                                    Debug.Fail("The stack is empty");
                            }
                            break;
                        case "TAB":
                            {
                                if (_data.Count > 0)
                                {
                                    var x = _data.Pop();
                                    //TODO: Type checking
                                    var count = (int)x.GetValueAsType<IntValue>();
                                    var result = new String(' ', count);
                                    _data.Push(new StringValue(result));
                                }
                                else
                                    Debug.Fail("The stack is empty");
                            }
                            break;
                        case "#WRITE":
                            {
                                if (_data.Count > 0)
                                {
                                    var value = _data.Pop();
                                    _terminal.Write(value.ToString() ?? "");
                                }
                                else
                                    Debug.Fail("The stack is empty");
                            }
                            break;
                        default:
                            //todo :error handling.
                            throw new NotImplementedException("The procedure has either not been implemented or there's another bug");
                    }
                    break;
                default:
                    //todo: error handling or remove this.
                    throw new NotImplementedException("The operation has either not been implemented or there's another bug");
            }
        }

        if (!EndMessageDisplayed && !_empty)
            End("Program has run through.");
        else if (_empty)
            _terminal.State = TerminalState.Empty;
    }

    void End(string message)
    {
        EndMessageDisplayed = true;

        if (_terminal != null)
        {
            if (_terminal.FullScreen)
                _terminal.FullScreen = false;

            _terminal.Title = "Simple direct mode";

            _terminal.WriteLine();
            _terminal.WriteLine(TheProgramHasEnded);

            if (!string.IsNullOrWhiteSpace(message))
                _terminal.WriteLine(message);

            _terminal.WriteLine();
            _terminal.WriteLine("Ready. Type RESTART, SOURCE, LOAD or QUIT.");
            _terminal.End();
        }
    }
}