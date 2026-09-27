using A_BASIC_Language.Language.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_BASIC_Language.Tests;

[TestClass]
public class StatementDiagnosticTests
{
    [DataTestMethod]
    [DataRow("HEJ", true)]
    [DataRow("PINT 123", true)]
    [DataRow("GARBAGE 1=2", true)]
    [DataRow("10 HEJ", false)]
    [DataRow("10 PRINT 1:HEJ", false)]
    [DataRow("10 IF 1 THEN HEJ", false)]
    public void UnknownStatementsDoNotReportMissingLetEquals(string source, bool direct)
    {
        var result = new BasicParser(source, direct).Result;
        Assert.IsFalse(result.Success);
        var errors = string.Join("; ", result.Errors);
        Assert.IsFalse(errors.Contains("'LET'"), errors);
        Assert.IsTrue(result.Errors.Count > 0);
    }

    [DataTestMethod]
    [DataRow("HEJ")]
    [DataRow("PINT 123")]
    [DataRow("GARBAGE 1=2")]
    public void UnknownCommandIsNamedInDiagnostic(string source)
    {
        var result = new BasicParser(source, direct: true).Result;
        StringAssert.Contains(string.Join("; ", result.Errors), "Unknown statement '");
    }

    [DataTestMethod]
    [DataRow("A=1")]
    [DataRow("A$ = \"TEXT\"")]
    [DataRow("A % = 2")]
    [DataRow("A(1, B(2)) = 3")]
    [DataRow("LET A=1")]
    [DataRow("LET A$=\"TEXT\"")]
    public void ValidAssignmentsStillParse(string source)
    {
        var result = new BasicParser(source, direct: true).Result;
        Assert.IsTrue(result.Success, string.Join("; ", result.Errors));
    }

    [TestMethod]
    public void ExplicitLetRetainsSpecificDiagnostic()
    {
        var result = new BasicParser("LET A 123", direct: true).Result;
        Assert.IsFalse(result.Success);
        StringAssert.Contains(string.Join("; ", result.Errors), "Expected equal sign in 'LET' statement");
    }
}
