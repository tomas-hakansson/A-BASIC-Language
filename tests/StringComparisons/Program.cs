using System;
using System.Collections.Generic;
using System.Linq;
using A_BASIC_Language.Language;
using A_BASIC_Language.Language.ValueTypes;
using A_BASIC_Language.Language.SpecificExecutors;
using A_BASIC_Language.ValueTypes;

// Dependency-free regression checks: dotnet run --project tests/StringComparisons
var operators = new (string Symbol, Func<double, double, bool> Compare)[]
{
    ("=", (a, b) => Math.Abs(a - b) < 0.00001),
    ("<>", (a, b) => Math.Abs(a - b) > 0.00001),
    ("<", (a, b) => a < b), (">", (a, b) => a > b),
    ("<=", (a, b) => a <= b), (">=", (a, b) => a >= b)
};
foreach (var op in operators)
{
    foreach (var pair in new[] { ("HEJ", "HEJ"), ("HEJ", "NEJ"), ("hej", "HEJ"),
        ("", ""), ("", "HEJ"), ("01", "1"), ("10", "2") })
    {
        var parsed = new Parser($"10 IF A${op.Symbol}\"{pair.Item2}\" THEN 100\n100 END").Result;
        Require(parsed.Success, string.Join("; ", parsed.Errors));
        Require(parsed.EvalValues.OfType<ABL_Procedure>().Any(p => p.Name == op.Symbol), "Missing comparison instruction");
        Require(parsed.EvalValues.OfType<ABL_Procedure>().Any(p => p.Name == "GOTO"), "Missing implicit GOTO");
        Check(new StringValue(pair.Item1), new StringValue(pair.Item2), op.Compare,
            op.Compare(string.Compare(pair.Item1, pair.Item2, StringComparison.OrdinalIgnoreCase), 0));
    }
    Check(new IntValue(5), new IntValue(5), op.Compare, op.Compare(5, 5));
    Check(new FloatValue(4.5), new IntValue(5), op.Compare, op.Compare(4.5, 5));
}
// Explicit expectations ensure case variants remain equal for every operator.
var caseInsensitiveResults = new[] { true, false, false, false, true, true };
for (var i = 0; i < operators.Length; i++)
{
    Check(new StringValue("hej"), new StringValue("HEJ"), operators[i].Compare, caseInsensitiveResults[i]);
    Check(new StringValue("HEJ"), new StringValue("hej"), operators[i].Compare, caseInsensitiveResults[i]);
}
Require(new Parser("10 IF A=5 THEN 100\n100 END").Result.Success, "Numeric IF failed");
Require(new Parser("10 IF \"HEJ\"=A$ THEN 100\n100 END").Result.Success, "Reversed string IF failed");
foreach (var reversed in new[] { false, true })
{
    try
    {
        Check(reversed ? new IntValue(5) : new StringValue("5"),
            reversed ? new StringValue("5") : new IntValue(5), operators[0].Compare, true);
        throw new Exception("Mixed types should fail");
    }
    catch (InvalidOperationException ex) when (ex.Message == "Type mismatch.") { }
}
Console.WriteLine("All string comparison regression checks passed.");

// Statement and grammar keywords do not require separating whitespace.
foreach (var source in new[] {
    "10 FORA=1TO10\n20 PRINTA\n30 NEXTA",
    "10 FORA=1TO10STEP2\n20 PRINTA\n30 NEXTA",
    "10 IFA=1THENPRINT\"YES\"\n20 END",
    "10 LETA=1\n20 GOTO30\n30 END"
})
{
    var result = new Parser(source).Result;
    Require(result.Success, source + ": " + string.Join("; ", result.Errors));
}
Console.WriteLine("All compact keyword parsing checks passed.");

static void Check(ValueBase left, ValueBase right, Func<double, double, bool> compare, bool expected)
{
    var stack = new Stack<ValueBase>();
    stack.Push(left);
    stack.Push(right);
    new ComparisonExecutor(stack).Run(10, compare);
    Require(stack.Count == 1, "Invalid result stack");
    Require((double)stack.Pop().GetValueAsType<FloatValue>() == (expected ? -1 : 0), "Wrong comparison result");
}
static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
