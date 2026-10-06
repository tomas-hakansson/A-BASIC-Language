#nullable enable
using System;
using System.Collections.Generic;
using A_BASIC_Language.Language.SpecificExecutors;
using A_BASIC_Language.Language.ValueTypes;
using A_BASIC_Language.SpecificExecutors;
using A_BASIC_Language.StringManipulation;
using A_BASIC_Language.ValueTypes;
using TerminalMatrixNetFramework;

namespace A_BASIC_Language.Language;

public class Interpreter
{
    private readonly Log _log;
    private readonly bool _runtime;
    private readonly bool _empty;
    const string TheProgramHasEnded = "The program has ended";
    IBasicTerminal? _terminal;
    readonly ParseResult _parseResult;
    readonly Dictionary<string, ValueBase>? _variables;//Ponder: do the value need to be nullable?
    readonly Dictionary<string, Dimension> _dimVariables;
    readonly Stack<ValueBase> _data;
    bool EndMessageDisplayed { get; set; }
    Random _random;//Note: For the RND function.
    int _currentLineNumber;
    public bool UserBreak { get; set; }

    public Interpreter(string source, bool runtime, Log log, RuntimeState? state = null)
    {
        _log = log;
        _runtime = runtime;
        _empty = string.IsNullOrWhiteSpace(source);
        var parser = new Parser(source, direct: !runtime);
        _parseResult = parser.Result;
        _variables = state == null ? new Dictionary<string, ValueBase>() : state.Variables;
        _dimVariables = state.Arrays;
        _data = new Stack<ValueBase>();
        _random = new Random();
        _currentLineNumber = 0;
    }

    public void Run(TerminalMatrixControl terminal) =>
        Run(new TerminalAdapter(terminal));

    public void Run(IBasicTerminal terminal)
    {
        _terminal = terminal;

        if (!_parseResult.Success)
        {
            End("?Syntax error: " + string.Join("; ", _parseResult.Errors));
            _log.Write("Syntax error: " + string.Join("; ", _parseResult.Errors));
            return;
        }
        try
        {
            Eval();
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or ArithmeticException or NotImplementedException)
        {
            _log.Write("Error, check terminal window.");
            End(_runtime
                ? $"?Error in line {_currentLineNumber}: {ex.Message}"
                : $"?Error: {ex.Message}");
        }
    }

    private void Fail(string message) =>
        throw new InvalidOperationException(message);

    void Eval()
    {
        EndMessageDisplayed = false;

        if (_terminal == null)
            throw new SystemException("Terminal not initialized.");

        _terminal.PumpEvents();

        var addExecutor = new AddExecutor(_data);
        var subtractExecutor = new SubtractExecutor(_data);
        var comparisonExecutor = new ComparisonExecutor(_data);
        var flatVariableExecutor = new FlatVariableExecutor(_data, Fail, _variables ?? new Dictionary<string, ValueBase>());
        var dimExecutor = new DimExecutor(_data, Fail, _dimVariables);

        _terminal.PumpEvents();

        var endProgram = false;
        var matchingNext = MatchLoops();
        var loops = new Stack<LoopFrame>();

        for (var i = 0; i < _parseResult.EvalValues.Count; i++)
        {
            _terminal.PumpEvents();

            if (endProgram || _terminal.QuitFlag)
            {
                _log.Write("Program ended.");
                break;
            }

            if (UserBreak)
            {
                _log.Write("User break.");
                break;
            }

            var e = _parseResult.EvalValues[i];
            _log.Write(e.ToString());

            switch (e)
            {
                case ABL_For loop:
                {
                    var step = Number(_data.Pop());
                    var limit = Number(_data.Pop());
                    var initial = Number(_data.Pop());

                    if (step == 0)
                        throw new InvalidOperationException("FOR STEP must not be zero.");

                    _variables?[loop.Symbol] = new FloatValue(initial);

                    if (step > 0 ? initial > limit : initial < limit)
                        i = matchingNext[i];
                    else
                        loops.Push(new LoopFrame(loop.Symbol, limit, step, i, matchingNext[i]));
                    break;
                }
                case ABL_Next next:
                {
                    if (loops.Count == 0)
                        throw new InvalidOperationException("NEXT without FOR.");
                    
                    var frame = loops.Peek();
                    
                    if (frame.End != i || (next.Symbol.Length > 0 && next.Symbol != frame.Symbol))
                        throw new InvalidOperationException("NEXT does not match active FOR.");
                    
                    var value = Number(_variables?[frame.Symbol]!) + frame.Step;
                    _variables?[frame.Symbol] = new FloatValue(value);
                    
                    if (frame.Step > 0 ? value <= frame.Limit : value >= frame.Limit)
                        i = frame.Start;
                    else
                        loops.Pop();
                    break;
                }
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
                        case "#NEGATE":
                            _data.Push(new FloatValue(-Number(_data.Pop())));
                            break;
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
                                    Fail("Insufficient items on the stack");
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
                                    Fail("Insufficient items on the stack");
                            }
                            break;
                        case "/":
                            {
                                if (_data.Count >= 2)
                                {
                                    var x = _data.Pop();
                                    var y = _data.Pop();

                                    //TODO: Type checking
                                    var divisor = Number(x);
                                    if (divisor == 0) throw new DivideByZeroException("Division by zero.");
                                    var result = Number(y) / divisor;
                                    _data.Push(new FloatValue(result));
                                }
                                else
                                {
                                    Fail("Insufficient items on the stack");
                                }
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
                        case "ABS":
                            {
                                if (_data.Count > 0)
                                {
                                    var number = _data.Pop();
                                    var asDouble = (double)number.GetValueAsType<FloatValue>();
                                    _data.Push(new FloatValue(Math.Abs(asDouble)));
                                }
                                else
                                {
                                    Fail("The stack is empty");
                                }
                            }
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
                                        Fail($"Undefined line {(int)label.GetValueAsType<IntValue>()}.");
                                    }
                                }
                                else
                                {
                                    Fail("The stack is empty");
                                }
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
                                            Fail($"Undefined line {(int)label.GetValueAsType<IntValue>()}.");
                                        }
                                    }
                                }
                                else
                                {
                                    Fail("The stack is empty");
                                }
                            }
                            break;
                        case "#INPUT-INT":
                            {
                                bool happy;
                                do
                                {
                                    if (_terminal.QuitFlag || UserBreak)
                                        return;

                                    happy = false;
                                    var value = ValueBase.GetValueType(_terminal.InputString(""));

                                    if (value.CanGetAsType<IntValue>())
                                    {
                                        var intValue = new IntValue((int)value.GetValueAsType<IntValue>());

                                        if (!_terminal.QuitFlag && !UserBreak)
                                            _data.Push(intValue);

                                        happy = true;
                                    }
                                    else
                                    {
                                        if (!_terminal.QuitFlag && !UserBreak)
                                        {
                                            _terminal.WriteLine("?Redo from start"); // TODO await?
                                            _terminal.Write("Enter a numeric value: "); // TODO await?
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }

                                } while (!happy);
                            }
                            break;
                        case "#INPUT-FLOAT":
                            {
                                bool happy;

                                do
                                {
                                    if (_terminal.QuitFlag || UserBreak)
                                        return;

                                    happy = false;
                                    var value = ValueBase.GetValueType(_terminal.InputString(""));

                                    if (value.CanGetAsType<FloatValue>())
                                    {
                                        var floatValue = new FloatValue((double)value.GetValueAsType<FloatValue>());

                                        if (!_terminal.QuitFlag && !UserBreak)
                                            _data.Push(floatValue);

                                        happy = true;
                                    }
                                    else
                                    {
                                        if (!_terminal.QuitFlag && !UserBreak)
                                        {
                                            _terminal.WriteLine("?Redo from start"); // TODO: Await?
                                            _terminal.Write("Enter a numeric value: "); // TODO: Await?
                                        }
                                        else
                                            return;
                                    }

                                } while (!happy);
                            }
                            break;
                        case "#INPUT-STRING":
                            {
                                if (_terminal.QuitFlag || UserBreak)
                                    return;

                                var value = new StringValue(_terminal.InputString(""));

                                if (!_terminal.QuitFlag && !UserBreak)
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
                            _terminal.WriteLine("");
                            break;
                        case "#NEXT-TAB-POSITION":
                            _terminal.Write(new string(' ', 14 - _terminal.OutputColumn % 14));
                            break;
                        case "RANDOMIZE":
                            _random = new Random();
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
                                    Fail("The stack is empty");
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
                                {
                                    Fail("The stack is empty");
                                }
                            }
                            break;
                        case "TAB":
                            {
                                if (_data.Count > 0)
                                {
                                    var x = _data.Pop();
                                    //TODO: Type checking
                                    var count = (int)x.GetValueAsType<IntValue>();
                                    var result = new string(' ', count);
                                    _data.Push(new StringValue(result));
                                }
                                else
                                {
                                    Fail("The stack is empty");
                                }
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
                                {
                                    Fail("The stack is empty");
                                }
                            }
                            break;
                        default:
                            //todo :error handling.
                            throw new NotImplementedException($"Unsupported BASIC function: {p.Name}");
                    }
                    break;
                default:
                    //todo: error handling or remove this.
                    throw new NotImplementedException("The operation has either not been implemented or there's another bug");
            }
        }

        if (!EndMessageDisplayed && !_empty && !_terminal.QuitFlag)
        {
            if (UserBreak)
            {
                UserBreak = false;
                End("User break.");
            }
            else if (_runtime)
            {
                End("Program has run through.");
            }
            else
            {
                End("");
            }
        }
    }

    //private sealed record LoopFrame(string Symbol, double Limit, double Step, int Start, int End);

    private sealed record LoopFrame
    {
        public string Symbol { get; }
        public double Limit { get; }
        public double Step { get; }
        public int Start { get; }
        public int End { get; }

        public LoopFrame(string symbol, double limit, double step, int start, int end)
        {
            Symbol = symbol;
            Limit = limit;
            Step = step;
            Start = start;
            End = end;
        }
    }


    private Dictionary<int, int> MatchLoops()
    {
        var pending = new PendingSymbolsStack();
        var result = new Dictionary<int, int>();

        for (var i = 0; i < _parseResult.EvalValues.Count; i++)
        {
            if (_parseResult.EvalValues[i] is ABL_For start)
            {
                pending.Push((i, start.Symbol));
            }
            else if (_parseResult.EvalValues[i] is ABL_Next end)
            {
                if (!pending.TryPop(out var loop) || (end.Symbol.Length > 0 && end.Symbol != loop.Symbol))
                    throw new InvalidOperationException("NEXT without matching FOR.");

                result.Add(loop.Index, i);
            }
        }

        if (pending.Count > 0)
            throw new InvalidOperationException("FOR without NEXT.");

        return result;
    }

    private static double Number(ValueBase value)
    {
        if (value is StringValue || !value.CanGetAsType<FloatValue>())
            throw new InvalidOperationException("Type mismatch: expected a number.");

        return (double)value.GetValueAsType<FloatValue>();
    }

    void End(string message)
    {
        EndMessageDisplayed = true;

        if (_terminal == null)
            return;

        if (!message.IsEmpty())
        {
            _terminal.WriteLine("");
            _terminal.WriteLine(string.IsNullOrWhiteSpace(message) ? TheProgramHasEnded : message);
        }

        _terminal.WriteLine("");
        _terminal.WriteLine("Ready.");
    }
}
