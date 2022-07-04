using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language.Stage0;
using System.Collections.Generic;

namespace A_BASIC_Language_Tests;

[TestClass]
public class Stage1Tokenizer_Tests
{
    [TestMethod]
    public void SimpleOneLineProgram()
    {
        var source =
@"10 input n";
        Tokenizer tokenizer = new(source);
        var tokenizedSource = tokenizer.TokenizedSource;
        List<string> ts_expected = new() { "10", "INPUT", "N" };
        CollectionAssert.AreEqual(ts_expected, tokenizedSource);
        var tokenizedLines = tokenizer.TokenizedLines;
        List<List<string>> tl_expected = new List<List<string>>()
        { new List<string>() { "10", "INPUT", "N" } };
        Assert.AreEqual(tl_expected.Count, tokenizedLines.Count);
        for (int i = 0; i < tl_expected.Count; i++)
        {
            CollectionAssert.AreEqual(tl_expected[i], tokenizedLines[i]);
        }
    }

    [TestMethod]
    public void SimpleTwoLineProgram()
    {
        var source =
@"10 input n
20 print n";
        Tokenizer tokenizer = new(source);
        var tokenizedSource = tokenizer.TokenizedSource;
        List<string> ts_expected = new() { "10", "INPUT", "N", "\n", "20", "PRINT", "N" };
        CollectionAssert.AreEqual(ts_expected, tokenizedSource);
        var tokenizedLines = tokenizer.TokenizedLines;
        List<List<string>> tl_expected = new List<List<string>>()
        { new List<string>() { "10", "INPUT", "N" },
          new List<string>() { "20", "PRINT", "N" } };
        Assert.AreEqual(tl_expected.Count, tokenizedLines.Count);
        for (int i = 0; i < tl_expected.Count; i++)
        {
            CollectionAssert.AreEqual(tl_expected[i], tokenizedLines[i]);
        }
    }

    [TestMethod]
    public void SimpleTwoLineProgram_SansSpace()
    {
        var source =
@"10inputn
20printn";
        Tokenizer tokenizer = new(source);
        var tokenizedSource = tokenizer.TokenizedSource;
        List<string> ts_expected = new() { "10", "INPUT", "N", "\n", "20", "PRINT", "N" };
        CollectionAssert.AreEqual(ts_expected, tokenizedSource);
        var lines = tokenizer.TokenizedLines;
        var tokenizedLines = tokenizer.TokenizedLines;
        List<List<string>> tl_expected = new List<List<string>>()
        { new List<string>() { "10", "INPUT", "N" },
          new List<string>() { "20", "PRINT", "N" } };
        Assert.AreEqual(tl_expected.Count, tokenizedLines.Count);
        for (int i = 0; i < tl_expected.Count; i++)
        {
            CollectionAssert.AreEqual(tl_expected[i], tokenizedLines[i]);
        }
    }

    [TestMethod]
    public void SimpleTwoLineProgram_WithExtraNewlines()
    {
        var source =
@"10 input n


20 print n
";
        Tokenizer tokenizer = new(source);
        var tokenizedSource = tokenizer.TokenizedSource;
        List<string> ts_expected = new() { "10", "INPUT", "N", "\n", "20", "PRINT", "N", "\n" };
        CollectionAssert.AreEqual(ts_expected, tokenizedSource);
        var tokenizedLines = tokenizer.TokenizedLines;
        List<List<string>> tl_expected = new List<List<string>>()
        { new List<string>() { "10", "INPUT", "N" },
          new List<string>() { "20", "PRINT", "N" } };
        Assert.AreEqual(tl_expected.Count, tokenizedLines.Count);
        for (int i = 0; i < tl_expected.Count; i++)
        {
            CollectionAssert.AreEqual(tl_expected[i], tokenizedLines[i]);
        }
    }

    [TestMethod]
    public void SimpleTwoLineProgram_WithParentheses_WithSingleCharacterTokens()
    {
        var source =
@"10 i=sqr(sqr(n))
20 j=(i^2)^2
";
        Tokenizer tokenizer = new(source);
        var tokenizedSource = tokenizer.TokenizedSource;
        List<string> ts_expected = new()
        {
            "10",
            "I",
            "=",
            "SQR",
            "(",
            "SQR",
            "(",
            "N",
            ")",
            ")",
            "\n",
            "20",
            "J",
            "=",
            "(",
            "I",
            "^",
            "2",
            ")",
            "^",
            "2",
            "\n"
        };
        CollectionAssert.AreEqual(ts_expected, tokenizedSource);
        var tokenizedLines = tokenizer.TokenizedLines;
        List<List<string>> tl_expected = new List<List<string>>()
        { new List<string>() { "10", "I", "=", "SQR", "(", "SQR", "(", "N", ")", ")" },
          new List<string>() {  "20", "J", "=", "(", "I", "^", "2", ")", "^", "2" } };
        Assert.AreEqual(tl_expected.Count, tokenizedLines.Count);
        for (int i = 0; i < tl_expected.Count; i++)
        {
            CollectionAssert.AreEqual(tl_expected[i], tokenizedLines[i]);
        }
    }

    [TestMethod]
    public void RemShouldNotBeTokenized()
    {
        var source =
@"10 rem this shouldn't be tokenized";
        Tokenizer tokenizer = new(source);
        var tokenizedSource = tokenizer.TokenizedSource;
        List<string> ts_expected = new() { "10", "REM", "\n" };
        CollectionAssert.AreEqual(ts_expected, tokenizedSource);
        var tokenizedLines = tokenizer.TokenizedLines;
        List<List<string>> tl_expected = new List<List<string>>()
        { new List<string>() { "10", "REM" } };
        Assert.AreEqual(tl_expected.Count, tokenizedLines.Count);
        for (int i = 0; i < tl_expected.Count; i++)
        {
            CollectionAssert.AreEqual(tl_expected[i], tokenizedLines[i]);
        }
    }

    [TestMethod]
    public void MostOfTheseShouldBeComments()
    {
        var source =
@"this doesn't begin with a 10 followed by a statement or dot so is a comment.
10 rem this shouldn't be tokenized
1. this is also a comment but the next one wont be
20 input n
30 print n";
        Tokenizer tokenizer = new(source);
        var tokenizedSource = tokenizer.TokenizedSource;
        List<string> ts_expected = new() { "10", "REM", "\n", "20", "INPUT", "N", "\n", "30", "PRINT", "N" };
        CollectionAssert.AreEqual(ts_expected, tokenizedSource);
        var tokenizedLines = tokenizer.TokenizedLines;
        List<List<string>> tl_expected = new List<List<string>>()
        { new List<string>() { "10", "REM" },
          new List<string>() { "20", "INPUT", "N" },
          new List<string>() { "30", "PRINT", "N" } };
        Assert.AreEqual(tl_expected.Count, tokenizedLines.Count);
        for (int i = 0; i < tl_expected.Count; i++)
        {
            CollectionAssert.AreEqual(tl_expected[i], tokenizedLines[i]);
        }
    }
}
