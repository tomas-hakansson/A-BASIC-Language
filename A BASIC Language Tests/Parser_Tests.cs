﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_BASIC_Language;
using A_BASIC_Language.Parsing;

namespace A_BASIC_Language_Tests;

[TestClass]
public class Parser_Tests
{
    [TestMethod]
    public void PRINT_OneValue()
    {
        var source = "10 print 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) P(#WRITE) P(#NEXT-LINE)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void PRINT_TwoValues()
    {
        var source = "10 print 42, \"hello\" ";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) P(#WRITE) P(#NEXT-TAB-POSITION) S(hello) P(#WRITE) P(#NEXT-LINE)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void PRINT_TwoValues_SansNewline()
    {
        var source = "10 print 42, \"hello\";";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) P(#WRITE) P(#NEXT-TAB-POSITION) S(hello) P(#WRITE)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void GOTO_SimpleValue()
    {
        var source = "10 goto 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) P(GOTO)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void GOTO_Expression()
    {
        var source = "10 goto 1 + 3";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(3) P(+) P(GOTO)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void GOTO_StopsParsingTheRestOfTheLine()
    {
        var source = "10 goto 42:print \"This won't be parsed\" ";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) P(GOTO)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void GO_TO_WorksToo()
    {
        var source = "10 go to 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) P(GOTO)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void LET_FullSyntax()
    {
        var source = "10 let x = 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) =(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void LET_SimpleFloatValue()
    {
        var source = "10 x = 4.2";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(4.2) =(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void LET_SimpleIntValue()
    {
        var source = "10 x% = 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(42) =(X%)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void LET_SimpleStringValue()
    {
        var source = "10 x$ = \"hello\" ";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(hello) =(X$)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void LET_SimpleExpression()
    {
        var source = "10 x = 40+2";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(40) N(2) P(+) =(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void REM_oves()
    {
        var source = "10 removes the rest of the line";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void INPUT_Float()
    {
        var source = "10 input x";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-FLOAT) =(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void INPUT_Int()
    {
        var source = "10 input x%";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-INT) =(X%)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void INPUT_String()
    {
        var source = "10 input x$";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-STRING) =(X$)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void INPUT_WithPrompt()
    {
        var source = "10 input \"enter float\"; x";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(enter float) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-FLOAT) =(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void INPUT_IntAndString()
    {
        var source = "10 input x%, y$";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-INT) =(X%) P(#INPUT-STRING) =(Y$)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void INPUT_IntAndString_WithPrompt()
    {
        var source = "10 input \"enter an int followed by a string\"; x%, y$";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(enter an int followed by a string) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-INT) =(X%) P(#INPUT-STRING) =(Y$)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void COLON_WorksWith_PRINT()
    {
        var source = "10 print:print \"tada\":print";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) P(#NEXT-LINE) S(tada) P(#WRITE) P(#NEXT-LINE) P(#NEXT-LINE)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void COLON_WorksWith_LET()
    {
        var source = "10 x = 4.2:let y$ = \"hello\":z%=42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(4.2) =(X) S(hello) =(Y$) N(42) =(Z%)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void COLON_WorksWith_GOTO()
    {
        var source = "10 print \"hello\";:goto 42:print\"world\":z%=42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(hello) P(#WRITE) N(42) P(GOTO)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void COLON_WorksWith_INPUT()
    {
        var source = "10 input x:input \"hi\"; y$";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(? ) P(#WRITE) P(#INPUT-FLOAT) =(X) S(hi) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-STRING) =(Y$)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void IF_SingleBranch_SimpleLabel()
    {
        var source = "10 if 1 then 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", values);
        Assert.AreEqual("(-2, 9) ++ (-1, 8) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_SingleBranch_ExpressionLabel()
    {
        var source = "10 if 1 then 40 + 2";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(40) N(2) P(+) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", values);
        Assert.AreEqual("(-2, 11) ++ (-1, 10) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_SingleBranch_SimpleStatement()
    {
        var source = "10 if 1 then print 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(#WRITE) P(#NEXT-LINE) N(-2) P(GOTO) L(-1) L(-2)", values);
        Assert.AreEqual("(-2, 10) ++ (-1, 9) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_SingleBranch_TWoStatements()
    {
        var source = "10 if 1 then print 42:x=4";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(#WRITE) P(#NEXT-LINE) N(4) =(X) N(-2) P(GOTO) L(-1) L(-2)", values);
        Assert.AreEqual("(-2, 12) ++ (-1, 11) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_SingleBranch_GOTOStatement()
    {
        var source = "10 if 1 then goto 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", values);
        Assert.AreEqual("(-2, 9) ++ (-1, 8) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BooleanComparisonWorks()
    {
        var source = "10 if x = 3 then 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) V(X) N(3) P(=) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", values);
        Assert.AreEqual("(-2, 11) ++ (-1, 10) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BooleanComparisonWorks_StringVariable()
    {
        var source = "10 if x$ = \"3\" then 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) V(X$) S(3) P(=) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2)", values);
        Assert.AreEqual("(-2, 11) ++ (-1, 10) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BothBranches_SimpleLabels()
    {
        var source = "10 if 1 then 42 else 666";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) N(666) P(GOTO) L(-2)", values);
        Assert.AreEqual("(-2, 11) ++ (-1, 8) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BothBranches_ExpressionLabels()
    {
        var source = "10 if 1 then 4+2 else 666 - 1";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(4) N(2) P(+) P(GOTO) N(-2) P(GOTO) L(-1) N(666) N(1) P(-) P(GOTO) L(-2)", values);
        Assert.AreEqual("(-2, 15) ++ (-1, 10) ++ (10, 0)", indices);
    }


    [TestMethod]
    public void IF_BothBranches_SimpleStatements()
    {
        var source = "10 if 1 then print 42; else print 666;";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(#WRITE) N(-2) P(GOTO) L(-1) N(666) P(#WRITE) L(-2)", values);
        Assert.AreEqual("(-2, 11) ++ (-1, 8) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BothBranches_SimpleGOTO()
    {
        var source = "10 if 1 then goto 42 else goto 666";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) N(666) P(GOTO) L(-2)", values);
        Assert.AreEqual("(-2, 11) ++ (-1, 8) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BothBranches_SeveralPRINTs()
    {
        var source = "10 if 1 then print 1;:print 2; else print 1;:print 2;";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(1) P(#WRITE) N(2) P(#WRITE) N(-2) P(GOTO) L(-1) N(1) P(#WRITE) N(2) P(#WRITE) L(-2)", values);
        Assert.AreEqual("(-2, 15) ++ (-1, 10) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BothBranches_SeveralLETs()
    {
        var source = "10 if 1 then x = 4: y = 5 else x = 4: y = 5";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(4) =(X) N(5) =(Y) N(-2) P(GOTO) L(-1) N(4) =(X) N(5) =(Y) L(-2)", values);
        Assert.AreEqual("(-2, 15) ++ (-1, 10) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BothBranches_DifferentStatementsInEach()
    {
        var source = "10 if 1 then goto 42 else print \"hello\" ";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) S(hello) P(#WRITE) P(#NEXT-LINE) L(-2)", values);
        Assert.AreEqual("(-2, 12) ++ (-1, 8) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void IF_BothBranches_SeveralStatements()
    {
        var source = "10 if 1 then print \"hello\";:goto 42 else print \"world\";:x=666:goto x";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(-1) P(#IF-FALSE-GOTO) S(hello) P(#WRITE) N(42) P(GOTO) N(-2) P(GOTO) L(-1) S(world) P(#WRITE) N(666) =(X) V(X) P(GOTO) L(-2)", values);
        Assert.AreEqual("(-2, 17) ++ (-1, 10) ++ (10, 0)", indices);
    }

    [TestMethod]
    public void TwoSimpleLines()
    {
        var source = @"
10 input ""enter name""; n$
20 print n$";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(enter name) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-STRING) =(N$) L(20) V(N$) P(#WRITE) P(#NEXT-LINE)", values);
        Assert.AreEqual("(10, 0) ++ (20, 7)", indices);
    }

    [TestMethod]
    public void LineNumbersDecidesOrder()
    {
        var source = @"
20 print n$
10 input ""enter name""; n$";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(enter name) P(#WRITE) S(? ) P(#WRITE) P(#INPUT-STRING) =(N$) L(20) V(N$) P(#WRITE) P(#NEXT-LINE)", values);
        Assert.AreEqual("(10, 0) ++ (20, 7)", indices);
    }

    [TestMethod]
    public void ALineThatStartsWithANumberButIsNotFollowedWithAStatementOrVariableIsAComment()
    {
        var source = @"
10. this is a comment
20 print 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(20) N(42) P(#WRITE) P(#NEXT-LINE)", values);
        Assert.AreEqual("(20, 0)", indices);
    }

    [TestMethod]
    public void DIM_Create_OneVariable_OneDimension()
    {
        var source = "10 dim a(4)";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(4) N(1) DC(A)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void DIM_Create_NestedIndex()
    {
        var source = "10 dim a(4, 1, x(b(4), 99))";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(4) N(1) N(4) N(1) DV(B) N(99) N(2) DV(X) N(3) DC(A)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void DIM_Create_TwoVariables_TwoDimensions_SomeVariables_WithTypeSpecifiers()
    {
        var source = "10 dim x$(1,4),y(a,b%, 3)";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(1) N(4) N(2) DC(X$) V(A) V(B%) N(3) N(3) DC(Y)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void DIM_ZeroDimensions()
    {
        //Note: A DIM with no dimensions becomes an ordinary variable.
        //  If that variable was set prior to this it's original value remains.
        var source = "10 dim x";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) V(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void DIM_LET()
    {
        var source = "10 x(3) = 42";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(3) N(1) N(42) D=(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void DIM_Accessing()
    {
        var source = "10 x = a(4)";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(4) N(1) DV(A) =(X)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void DIM_INPUT()
    {
        var source = "10 input a(4)";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) S(? ) P(#WRITE) N(4) N(1) P(#INPUT-FLOAT) D=(A)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void DIM_PRINT()
    {
        var source = "10 print a(4)";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) N(4) N(1) DV(A) P(#WRITE) P(#NEXT-LINE)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void BugFix_CorrectlyParseSQR()
    {
        var source = "10 i=sqr(sqr(n))";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) V(N) P(SQR) P(SQR) =(I)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void BugFix_CorrectlyParseExponentiationOperator()
    {
        var source = "10 j=(i^2)^2";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) V(I) N(2) P(^) N(2) P(^) =(J)", values);
        Assert.AreEqual("(10, 0)", indices);
    }

    [TestMethod]
    public void BugFix_IF_SometimesBreaksTheNextLine()
    {
        var source = @"10 IF K <= 0 THEN 42
20 N = N - K";
        (var values, string indices) = Parse(source);
        Assert.AreEqual("L(10) V(K) N(0) P(<=) N(-1) P(#IF-FALSE-GOTO) N(42) P(GOTO) N(-2) P(GOTO) L(-1) L(-2) L(20) V(N) V(K) P(-) =(N)", values);
        Assert.AreEqual("(-2, 11) ++ (-1, 10) ++ (10, 0) ++ (20, 12)", indices);
    }

    //[TestMethod]
    //public void BugFixTest_4_5_shouldBe_45()
    //{
    //    var source = "10 print 4 5;";
    //    Assert.AreEqual("L(10) N(45) P(#WRITE)", values);
    //}

    //[TestMethod]
    //public void DEF_something()//ToDo: def fn
    //{
    //    var source = "10 def fn f(x) = 2 * x";
    //    Assert.AreEqual("L(10) FN(f, x) N(2) V(X) P(*) P(RETURN)", values);
    //}

    private static (string values, string indices) Parse(string source)
    {
        ParseResult result = new Parser(source).Result;
        return (result.ToString(PrintThe.EvalValues), result.ToString(PrintThe.LabelIndex));
    }
}
