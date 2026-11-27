using System.Collections.Generic;

namespace A_BASIC_Language.Language;

public class InstructionList : List<AblEvalValue>
{
    public InstructionList()
    {
    }

    public InstructionList(IEnumerable<AblEvalValue> values) : base(values)
    {
    }

    public InstructionList(List<AblEvalValue> values)
    {
        AddRange(values);
    }


    public int NextInstruction { get; set; }
}
