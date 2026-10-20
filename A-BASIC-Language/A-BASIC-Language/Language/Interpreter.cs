#nullable enable
using A_BASIC_Language.Language.LanguageParts;
using A_BASIC_Language.Language.SpecificExecutors;
using A_BASIC_Language.Language.ValueTypes;
using A_BASIC_Language.SpecificExecutors;
using A_BASIC_Language.StringManipulation;
using System;
using System.Collections.Generic;
using System.IO;
using A_BASIC_Language.Language.LoopInstructions;
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
    readonly Dictionary<string, AblFunctionDefinition> _functions;
    bool EndMessageDisplayed { get; set; }
    Random _random;//Note: For the RND function.
    private readonly Stack<(int Index, int LineNumber)> _subroutineCalls = new();
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
        _dimVariables = state == null ? new Dictionary<string, Dimension>() : state.Arrays;
        _data = new Stack<ValueBase>();
        _functions = state == null ? new Dictionary<string, AblFunctionDefinition>(StringComparer.OrdinalIgnoreCase) : state.Functions;
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

    private void Save(string fileName)
    {
        var fi = new FileInfo(Path.Combine(MainWindow.UserDataPath, $"{fileName}.bas"));
        
    }

    void Eval()
    {
        EndMessageDisplayed = false;
        _subroutineCalls.Clear();

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

        var instructions = _parseResult.EvalValues;
        // Switch instruction streams on FN calls; return with one numeric result.
        var calls = new Stack<(List<AblEvalValue> Instructions, int Index, string Parameter, ValueBase Argument, int StackDepth)>();
        for (var i = 0; i < instructions.Count || calls.Count > 0; i++)
        {
            if (i >= instructions.Count)
            {
                var frame = calls.Pop();
                if (_data.Count != frame.StackDepth + 1)
                    throw new InvalidOperationException("Invalid FN result stack.");
                Number(_data.Peek());
                instructions = frame.Instructions;
                i = frame.Index;
                continue;
            }
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

            var e = instructions[i];
            _log.Write(e.ToString());

            switch (e)
            {
                case AblOnGoto branch:
                {
                    var selector = Number(_data.Pop());
                    if (double.IsNaN(selector) || double.IsInfinity(selector) || selector < 0)
                        throw new OverflowException(branch.IsSubroutine ? "ON GOSUB index is out of range." : "ON GOTO index is out of range.");

                    var selected = Math.Floor(selector);
                    if (selected >= 1 && selected <= branch.Targets.Count)
                    {
                        var target = branch.Targets[(int)selected - 1];
                        if (!_parseResult.LabelIndex.TryGetValue(target, out var targetIndex))
                            throw new InvalidOperationException($"Undefined line {target}.");
                        if (branch.IsSubroutine)
                            _subroutineCalls.Push((i + 1, _currentLineNumber));
                        i = targetIndex - 1;
                    }
                    break;
                }
                case AblFunctionDefinition definition:
                    _functions[definition.Name] = definition;
                    break;
                case AblFunctionCall call:
                {
                    if (!_functions.TryGetValue(call.Name, out var function))
                        throw new InvalidOperationException($"Undefined function FN {call.Name}.");
                    if (calls.Count >= 256)
                        throw new InvalidOperationException("FN call nesting limit exceeded.");
                    var argument = new FloatValue(Number(_data.Pop()));
                    calls.Push((instructions, i, function.Parameter, argument, _data.Count));
                    instructions = function.Body;
                    i = -1;
                    break;
                }
                case AblFor loop:
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
                case AblNext next:
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
                case AblLabel lbl:
                    _currentLineNumber = lbl.Value;
                    break;
                case AblNumber n:
                    _data.Push(ValueBase.GetValueType(n.Value));
                    break;
                case AblString s:
                    _data.Push(new StringValue(s.Value));
                    break;
                case ABL_DIM_Creation dc:
                    dimExecutor.Create(dc);
                    break;
                case ABL_Variable v:
                    // Only the current function's parameter shadows a global variable.
                    if (calls.Count > 0 && calls.Peek().Parameter == v.Symbol)
                        _data.Push(calls.Peek().Argument);
                    else
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
                case AblProcedure p:
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
                        case "EXP":
                            _data.Push(new FloatValue(Math.Exp(Number(_data.Pop()))));
                            break;
                        case "#END-PROGRAM":
                            endProgram = true;
                            break;
                        case "RETURN":
                            {
                                if (_subroutineCalls.Count == 0)
                                    throw new InvalidOperationException("RETURN without GOSUB.");
                                var returnPoint = _subroutineCalls.Pop();
                                _currentLineNumber = returnPoint.LineNumber;
                                i = returnPoint.Index - 1;
                            }
                            break;
                        case "GOSUB":
                            {
                                var target = Number(_data.Pop());
                                if (double.IsNaN(target) || double.IsInfinity(target) ||
                                    target < 0 || target > int.MaxValue)
                                    throw new InvalidOperationException("GOSUB line number is out of range.");
                                if (!_parseResult.LabelIndex.TryGetValue((int)target, out var targetIndex))
                                    throw new InvalidOperationException($"Undefined line {(int)target}.");
                                _subroutineCalls.Push((i + 1, _currentLineNumber));
                                i = targetIndex - 1;
                            }
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
                                        break;

                                    happy = false;
                                    var input = _terminal.InputString("");
                                    if (_terminal.QuitFlag || UserBreak)
                                        break;

                                    var value = ValueBase.GetValueType(input);

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
                                            break;
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
                                        break;

                                    happy = false;
                                    var input = _terminal.InputString("");

                                    if (_terminal.QuitFlag || UserBreak)
                                        break;

                                    var value = ValueBase.GetValueType(input);

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
                                            break;
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
                        case "WINDOW":
                            {
                                var rows = Number(_data.Pop());
                                if (double.IsNaN(rows) || double.IsInfinity(rows) ||
                                    rows < 2 || rows > 25 || rows != Math.Floor(rows))
                                    throw new InvalidOperationException("WINDOW requires a whole number of text rows from 2 to 25.");
                                _terminal.ClearScreen();
                                _terminal.SetTextRenderLimit(25 - (int)rows);
                            }
                            break;
                        case "RANDOMIZE":
                            _random = new Random();
                            break;
                        case "SAVE":
                            if (_runtime)
                                throw new InvalidOperationException("SAVE is only allowed in direct mode.");
                            if (_data.Count == 0 || _data.Pop() is not StringValue fileName)
                                throw new InvalidOperationException("SAVE requires a string argument.");
                            Save(fileName.Value);
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
                                    // BASIC TAB uses one-based absolute columns.
                                    var targetColumn = Math.Max(0, count - 1);
                                    if (targetColumn < _terminal.OutputColumn)
                                        _terminal.WriteLine("");
                                    var result = new string(' ', targetColumn - _terminal.OutputColumn);
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
                                    var text = value.ToString() ?? "";
                                    
                                    if (value is IntValue || value is FloatValue)
                                        text = (text.StartsWith("-", StringComparison.Ordinal) ? "" : " ") + text + " ";
                                    
                                    _terminal.Write(text);
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
                End("Program has run through.", showReady: false);
            }
            else
            {
                End("");
            }
        }

        if (_empty)
        {
            _terminal.WriteLine("");
            _terminal.WriteLine("Ready.");
        }
    }

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
            if (_parseResult.EvalValues[i] is AblFor start)
            {
                pending.Push((i, start.Symbol));
            }
            else if (_parseResult.EvalValues[i] is AblNext end)
            {
                if (!pending.TryPop(out var loop) || (end.Symbol.Length > 0 && end.Symbol != loop.Symbol))
                    throw new InvalidOperationException("NEXT without matching FOR.");

                result.Add(loop.Index, i);
            }
        }

        return pending.Count > 0 ? throw new InvalidOperationException("FOR without NEXT.") : result;
    }

    private static double Number(ValueBase value)
    {
        if (value is StringValue || !value.CanGetAsType<FloatValue>())
            throw new InvalidOperationException("Type mismatch: expected a number.");

        return (double)value.GetValueAsType<FloatValue>();
    }

    private void End(string message, bool showReady = true)
    {
        EndMessageDisplayed = true;

        if (_terminal == null)
            return;

        if (!message.IsEmpty())
        {
            _terminal.WriteLine("");
            _terminal.WriteLine(string.IsNullOrWhiteSpace(message) ? TheProgramHasEnded : message);
        }

        if (showReady)
        {
            _terminal.WriteLine("");
            _terminal.WriteLine("Ready.");
        }
    }
}
