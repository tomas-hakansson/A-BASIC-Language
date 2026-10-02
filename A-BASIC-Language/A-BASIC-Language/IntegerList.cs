#nullable enable
using System.Collections.Generic;

namespace A_BASIC_Language;

public class IntegerList : List<int>
{
    public IntegerList()
    {
    }

    public IntegerList(List<int> list)
    {
        AddRange(list);
    }

    public IEnumerable<int> TakeLast(int count)
    {
        if (count <= 0)
            yield break;

        // Använd Queue som en glidande buffert
        var queue = new Queue<int>(count);

        foreach (var item in this)
        {
            if (queue.Count == count)
                queue.Dequeue();

            queue.Enqueue(item);
        }

        foreach (var item in queue)
            yield return item;
    }
}
