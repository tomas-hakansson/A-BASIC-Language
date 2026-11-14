using System.Collections.Generic;
using System.Text;
using A_BASIC_Language.Language;

namespace A_BASIC_Language;

public class AblEvalValueList : List<AblEvalValue>
{
    public override string ToString()
    {
        var s = new StringBuilder();

        foreach (var value in this)
            s.AppendLine(value.ToString());

        return s.ToString();
    }
}
