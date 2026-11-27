using A_BASIC_Language.Language.ValueTypes;
using System.Collections.Generic;

namespace A_BASIC_Language.Language;

public class CallStack : Stack<(List<AblEvalValue> Instructions, int Index, string Parameter, ValueBase Argument, int StackDepth)>
{
}
