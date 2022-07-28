using System.Diagnostics;
using A_BASIC_Language.Gui;
using A_BASIC_Language.ValueTypes;

namespace A_BASIC_Language.StageN;

public class Interpreter
{
    private const string TheProgramHasEnded = "The program has ended";
    private Terminal? _terminal;
    readonly List<ABL_EvalValue> _evalValues;
    readonly Dictionary<int, int> _labelIndex;//todo: replace with ParsedProgram.
    readonly Dictionary<string, ValueBase?> _variables;//Ponder: do the value need to be nullable?
    readonly Stack<ValueBase> _data;

    int _index;

    public Interpreter(string source)
    {
        Stage1.Tokenizer t1 = new(source);
        StageN.Parser testParser = new(t1.TokenValues, t1.TokenTypes);
        _evalValues = testParser.ABL_EvalValues;
        _labelIndex = testParser.LabelIndex;
        _variables = new Dictionary<string, ValueBase?>();
        _data = new Stack<ValueBase>();

        _index = 0;
    }

    public void Run(Terminal terminal)
    {
        _terminal = terminal;

        //for (; _index < _lines.Count; _index++)
        {
          //  Application.DoEvents();

            //if (!_terminal.Running)
            //    return;

            Eval();
        }
    }

    private void Eval()
    {
        if (_terminal == null)
            throw new SystemException("Terminal not initialized.");

        Application.DoEvents();

        //todo: Debug.WriteLine($"Eval {_index}: {line}");

        for (int i = 0; i < _evalValues.Count; i++)
        {
            if (!_terminal.Running)
                return;

            switch (_evalValues[i])
            {
                case ABL_Label:
                    //Note: NOP.
                    break;
                case ABL_Number n:
                    _data.Push(ValueBase.GetValueType(n.Value));
                    break;
                case ABL_String s:
                    _data.Push(ValueBase.GetValueType(s.Value));
                    break;
                case ABL_Variable v:
                    {
                        if (_variables.TryGetValue(v.Symbol, out var value) && value != null)
                        {
                            _data.Push(value);
                        }
                        else
                        {
                            Debug.Fail("Something was wrong with the value");//fixme: really bad text.
                                                                             //todo: error handling.
                        }
                    }
                    break;
                case ABL_Assignment a:
                    {
                        if (_data.Count > 0)
                        {
                            var value = _data.Pop();
                            var symbol = a.Symbol;

                            if (!value.FitsInVariable(symbol))
                                End("Type mismatch."); // TODO: Better error message and also line number.

                            _variables[symbol] = value;
                        }
                        else
                            Debug.Fail("The stack is empty");
                    }
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
                        case "INPUT":
                            {
                                var value = ValueBase.GetValueType(_terminal.ReadLine()); // TODO: Must support "redo from start".
                                _data.Push(value);
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
                                    //TODO: Type checking
                                    if (_labelIndex.TryGetValue((int)label.GetValueAsType<IntValue>(), out var newIndex))
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

    private void End(string message)
    {
        if (_terminal != null)
        {
            _terminal.WriteLine("");
            _terminal.Write(TheProgramHasEnded);
            _terminal.End();
        }
        MessageBox.Show(message, TheProgramHasEnded);
    }
}
