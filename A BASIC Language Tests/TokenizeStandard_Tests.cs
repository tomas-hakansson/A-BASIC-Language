using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language.Stage0;
using System.Collections.Generic;

namespace A_BASIC_Language_Tests;

[TestClass]
public class TokenizeStandard_Tests
{
    [TestMethod]
    public void CanTokenizeOneInTheMiddle()
    {
        var source = "42 print nth";
        int index = 3;//the location of the token to be 'Read'.
        List<string> standardTokens = new() { "PRINT" };
        TokenizeStandard ts = new(source, standardTokens);
        var result = ts.Read(index);
        Assert.IsTrue(result.Success);
        Assert.AreEqual("PRINT", result.Token);
        //9 is the index of the first letter of the next (non-whitespace) token.
        Assert.AreEqual(9, result.NewIndex);
    }

    [TestMethod]
    public void SignalsEOF()
    {
        var source = "42 print";
        var index = 3;//the location of the token to be 'Read'.
        List<string> standardTokens = new() { "PRINT" };
        TokenizeStandard ts = new(source, standardTokens);
        var result = ts.Read(index);
        Assert.IsTrue(result.Success);
        Assert.AreEqual("PRINT", result.Token);
        Assert.IsTrue(result.EOF);
        //Since it's EOF the 'newIndex' is set to -1.
        Assert.AreEqual(-1, result.NewIndex);
    }

    [TestMethod]
    public void SignalsIsNewLine()
    {
        var source =
@"42 print
50 something else";
        var index = 3;//the location of the token to be 'Read'.
        List<string> standardTokens = new() { "PRINT" };
        TokenizeStandard ts = new(source, standardTokens);
        var result = ts.Read(index);
        Assert.IsTrue(result.Success);
        Assert.AreEqual("PRINT", result.Token);
        Assert.IsTrue(result.IsNewLine);
        Assert.IsFalse(result.EOF);
        //10 is the index of the first letter of the next (non-whitespace) token.
        Assert.AreEqual(10, result.NewIndex);
    }

    [TestMethod]
    public void CanHandleMultipleCalls()
    {
        var source = "42 print:input n";
        var index = 3;//the location of the first token to be 'Read'.
        List<string> standardTokens = new() { "PRINT", "INPUT", ":" };
        TokenizeStandard ts = new(source, standardTokens);
        var result1 = ts.Read(index);
        var result2 = ts.Read(result1.NewIndex);
        var result3 = ts.Read(result2.NewIndex);

        Assert.IsTrue(result3.Success);
        Assert.AreEqual("INPUT", result3.Token);
        Assert.IsFalse(result3.IsNewLine);
        Assert.IsFalse(result3.EOF);
        //15 is the index of the first letter of the next (non-whitespace) token.
        Assert.AreEqual(15, result3.NewIndex);
    }

    [TestMethod]
    public void CanHandleMultipleCallsWithExtraWhitespace()
    {
        var source = "42 print   :   input    n";
        var index = 3;//the location of the first token to be 'Read'.
        List<string> standardTokens = new() { "PRINT", "INPUT", ":" };
        TokenizeStandard ts = new(source, standardTokens);
        var result1 = ts.Read(index);
        var result2 = ts.Read(result1.NewIndex);
        var result3 = ts.Read(result2.NewIndex);

        Assert.IsTrue(result3.Success);
        Assert.AreEqual("INPUT", result3.Token);
        Assert.IsFalse(result3.IsNewLine);
        Assert.IsFalse(result3.EOF);
        //24 is the index of the first letter of the next (non-whitespace) token.
        Assert.AreEqual(24, result3.NewIndex);
    }

    [TestMethod]
    public void CanHandleMultipleCallsWithNoWhitespace()
    {
        var source = "42print:inputn";
        var index = 2;//the location of the first token to be 'Read'.
        List<string> standardTokens = new() { "PRINT", "INPUT", ":" };
        TokenizeStandard ts = new(source, standardTokens);
        var result1 = ts.Read(index);
        var result2 = ts.Read(result1.NewIndex);
        var result3 = ts.Read(result2.NewIndex);

        Assert.IsTrue(result3.Success);
        Assert.AreEqual("INPUT", result3.Token);
        Assert.IsFalse(result3.IsNewLine);
        Assert.IsFalse(result3.EOF);
        //24 is the index of the first letter of the next (non-whitespace) token.
        Assert.AreEqual(13, result3.NewIndex);
    }

    [TestMethod]
    public void ReturnsAnUnchangedStateIfNonStandardToken()
    {
        var source = "42 print variable";
        var index = 9;//the location of the first token to be 'Read'.
        List<string> standardTokens = new() { "PRINT", "INPUT", ":" };
        TokenizeStandard ts = new(source, standardTokens);
        var result = ts.Read(index);

        Assert.IsFalse(result.Success);
        Assert.AreEqual(string.Empty, result.Token);
        Assert.IsFalse(result.IsNewLine);
        Assert.IsFalse(result.EOF);
        //since it's a non-standard token it returns the given index.
        Assert.AreEqual(9, result.NewIndex);
    }

    [TestMethod]
    public void MustBeAbleToHandleNoWhitespaceBetweenTokens()
    {
        var source = "42printvariable";
        var index = 2;//the location of the first token to be 'Read'.
        List<string> standardTokens = new() { "PRINT", "INPUT", ":" };
        TokenizeStandard ts = new(source, standardTokens);
        var result = ts.Read(index);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("PRINT", result.Token);
        Assert.IsFalse(result.IsNewLine);
        Assert.IsFalse(result.EOF);
        Assert.AreEqual(7, result.NewIndex);
    }

    [TestMethod]
    public void MatchTheLongestMatchingStandardToken()
    {
        var source = "42 goto 20";
        var index = 3;//the location of the first token to be 'Read'.
        List<string> standardTokens = new() { "GO", "TO", "GOTO" };
        TokenizeStandard ts = new(source, standardTokens);
        var result = ts.Read(index);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("GOTO", result.Token);
        Assert.IsFalse(result.IsNewLine);
        Assert.IsFalse(result.EOF);
        Assert.AreEqual(8, result.NewIndex);
    }
}
