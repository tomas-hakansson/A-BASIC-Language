using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language;
using A_BASIC_Language.Stage2;

namespace A_BASIC_Language_Tests;

[TestClass]
public class Parser_Tests
{
    [TestMethod]
    public void PRINT_TwoValues()
    {
        var source = "10 print 42, \"hello\" ";
        Assert.AreEqual("L(10) N(42) P(#WRITE) P(#NEXT-TAB-POSITION) S(hello) P(#WRITE) P(#NEXT-LINE)", Parse(source));
    }

    [TestMethod]
    public void PRINT_TwoValues_SansNewline()
    {
        var source = "10 print 42, \"hello\";";
        Assert.AreEqual("L(10) N(42) P(#WRITE) P(#NEXT-TAB-POSITION) S(hello) P(#WRITE)", Parse(source));
    }

    [TestMethod]
    public void GOTO_SimpleValue()
    {
        var source = "10 goto 42";
        Assert.AreEqual("L(10) N(42) P(GOTO)", Parse(source));
    }

    [TestMethod]
    public void GOTO_Expression()
    {
        var source = "10 goto 1 + 3";
        Assert.AreEqual("L(10) N(1) N(3) P(+) P(GOTO)", Parse(source));
    }

    [TestMethod]
    public void GOTO_StopsParsingTheRestOfTheLine()
    {
        var source = "10 goto 42:print \"This won't be parsed\" ";
        Assert.AreEqual("L(10) N(42) P(GOTO)", Parse(source));
    }

    [TestMethod]
    public void GO_TO_WorksToo()
    {
        var source = "10 go to 42";
        Assert.AreEqual("L(10) N(42) P(GOTO)", Parse(source));
    }

    [TestMethod]
    public void LET_FullSyntax()
    {
        var source = "10 let x = 42";
        Assert.AreEqual("L(10) N(42) =(X)", Parse(source));
    }

    [TestMethod]
    public void LET_SimpleFloatValue()
    {
        var source = "10 x = 4.2";
        Assert.AreEqual("L(10) N(4.2) =(X)", Parse(source));
    }

    [TestMethod]
    public void LET_SimpleIntValue()
    {
        var source = "10 x% = 42";
        Assert.AreEqual("L(10) N(42) =(X%)", Parse(source));
    }

    [TestMethod]
    public void LET_SimpleStringValue()
    {
        var source = "10 x$ = \"hello\" ";
        Assert.AreEqual("L(10) S(hello) =(X$)", Parse(source));
    }

    [TestMethod]
    public void LET_SimpleExpression()
    {
        var source = "10 x = 40+2";
        Assert.AreEqual("L(10) N(40) N(2) P(+) =(X)", Parse(source));
    }

    [TestMethod]
    public void REM_oves()
    {
        var source = "10 removes the rest of the line";
        Assert.AreEqual("L(10)", Parse(source));
    }

    [TestMethod]
    public void INPUT_Float()
    {
        var source = "10 input x";
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-FLOAT) =(X)", Parse(source));
    }

    [TestMethod]
    public void INPUT_Int()
    {
        var source = "10 input x%";
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-INT) =(X%)", Parse(source));
    }

    [TestMethod]
    public void INPUT_String()
    {
        var source = "10 input x$";
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-STRING) =(X$)", Parse(source));
    }

    [TestMethod]
    public void INPUT_WithPrompt()
    {
        var source = "10 input \"enter float\"; x";
        Assert.AreEqual("L(10) S(enter float) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-FLOAT) =(X)", Parse(source));
    }

    [TestMethod]
    public void INPUT_IntAndString()
    {
        var source = "10 input x%, y$";
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-INT) =(X%) P(#INPUT-STRING) =(Y$)", Parse(source));
    }

    [TestMethod]
    public void INPUT_IntAndString_WithPrompt()
    {
        var source = "10 input \"enter an int followed by a string\"; x%, y$";
        Assert.AreEqual("L(10) S(enter an int followed by a string) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-INT) =(X%) P(#INPUT-STRING) =(Y$)", Parse(source));
    }

    [TestMethod]
    public void COLON_WorksWith_PRINT()
    {
        var source = "10 print:print \"tada\":print";
        Assert.AreEqual("L(10) P(#NEXT-LINE) S(tada) P(#WRITE) P(#NEXT-LINE) P(#NEXT-LINE)", Parse(source));
    }

    [TestMethod]
    public void COLON_WorksWith_LET()
    {
        var source = "10 x = 4.2:let y$ = \"hello\":z%=42";
        Assert.AreEqual("L(10) N(4.2) =(X) S(hello) =(Y$) N(42) =(Z%)", Parse(source));
    }

    [TestMethod]
    public void COLON_WorksWith_GOTO()
    {
        var source = "10 print \"hello\";:goto 42:print\"world\":z%=42";
        Assert.AreEqual("L(10) S(hello) P(#WRITE) N(42) P(GOTO)", Parse(source));
    }

    [TestMethod]
    public void COLON_WorksWith_INPUT()
    {
        var source = "10 input x:input \"hi\"; y$";
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-FLOAT) =(X) S(hi) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-STRING) =(Y$)", Parse(source));
    }

    [TestMethod]
    public void IF_SingleBranch_SimpleLabel()
    {
        var source = "10 if 1 then 42";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_SingleBranch_ExpressionLabel()
    {
        var source = "10 if 1 then 40 + 2";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(40) N(2) P(+) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_SingleBranch_SimpleStatement()
    {
        var source = "10 if 1 then print 42";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(#WRITE) P(#NEXT-LINE) N(-2) P(GOTO) L(-1) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_SingleBranch_TWoStatements()
    {
        var source = "10 if 1 then print 42:x=4";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(#WRITE) P(#NEXT-LINE) N(4) =(X) N(-2) P(GOTO) L(-1) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_SingleBranch_GOTOStatement()
    {
        var source = "10 if 1 then goto 42";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BooleanComparisonWorks()
    {
        var source = "10 if x = 3 then 42";
        Assert.AreEqual("L(10) V(X) N(3) P(=) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BooleanComparisonWorks_StringVariable()
    {
        var source = "10 if x$ = \"3\" then 42";
        Assert.AreEqual("L(10) V(X$) S(3) P(=) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BothBranches_SimpleLabels()
    {
        var source = "10 if 1 then 42 else 666";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) N(666) P(GOTO) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BothBranches_ExpressionLabels()
    {
        var source = "10 if 1 then 4+2 else 666 - 1";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(4) N(2) P(+) P(GOTO) N(-2) P(GOTO) L(-1) N(666) N(1) P(-) P(GOTO) L(-2)", Parse(source));
    }


    [TestMethod]
    public void IF_BothBranches_SimpleStatements()
    {
        var source = "10 if 1 then print 42; else print 666;";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(#WRITE) N(-2) P(GOTO) L(-1) N(666) P(#WRITE) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BothBranches_SimpleGOTO()
    {
        var source = "10 if 1 then goto 42 else goto 666";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) N(666) P(GOTO) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BothBranches_SeveralPRINTs()
    {
        var source = "10 if 1 then print 1;:print 2; else print 1;:print 2;";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(1) P(#WRITE) N(2) P(#WRITE) N(-2) P(GOTO) L(-1) N(1) P(#WRITE) N(2) P(#WRITE) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BothBranches_SeveralLETs()
    {
        var source = "10 if 1 then x = 4: y = 5 else x = 4: y = 5";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(4) =(X) N(5) =(Y) N(-2) P(GOTO) L(-1) N(4) =(X) N(5) =(Y) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BothBranches_DifferentStatementsInEach()
    {
        var source = "10 if 1 then goto 42 else print \"hello\" ";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) S(hello) P(#WRITE) P(#NEXT-LINE) L(-2)", Parse(source));
    }

    [TestMethod]
    public void IF_BothBranches_SeveralStatements()
    {
        var source = "10 if 1 then print \"hello\";:goto 42 else print \"world\";:x=666:goto x";
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) S(hello) P(#WRITE) N(42) P(GOTO) N(-2) P(GOTO) L(-1) S(world) P(#WRITE) N(666) =(X) V(X) P(GOTO) L(-2)", Parse(source));
    }

    [TestMethod]
    public void TwoSimpleLines()
    {
        var source = @"
10 input ""enter name""; n$
20 print n$";
        Assert.AreEqual("L(10) S(enter name) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-STRING) =(N$) L(20) V(N$) P(#WRITE) P(#NEXT-LINE)", Parse(source));
    }

    [TestMethod]
    public void DIM_OneVariable_OneDimension()
    {
        var source = "10 dim a(4)";
        Assert.AreEqual("L(10) N(4) N(1) P(#CREATE-ARRAY) =(A)", Parse(source));
    }

    [TestMethod]
    public void DIM_TwoVariables_TwoDimensions_SomeVariables_WithTypeSpecifiers()
    {
        var source = "10 dim x$(1,4),y(a,b%, 3)";
        Assert.AreEqual("L(10) N(1) N(4) N(2) P(#CREATE-ARRAY) =(X$) V(A) V(B%) N(3) N(3) P(#CREATE-ARRAY) =(Y)", Parse(source));
    }

    //[TestMethod]
    //public void DIM_Accessing()
    //{
    //    var source = "10 x = a(4)";
    //    Assert.AreEqual("L(10) N(4) N(1) P(#READ-ARRAY) =(X)", Parse(source));
    //}

    [TestMethod]
    //public void DEF_something()//ToDo: def fn
    //{
    //    var source = "10 def fn f(x) = 2 * x";
    //    Assert.AreEqual("L(10) FN(f, x) N(2) V(X) P(*) P(RETURN)", Parse(source));
    //}

    private static string Parse(string source) => new Parser(source).Result.ToString(PrintThe.EvalValues);
}
