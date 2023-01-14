namespace A_BASIC_Language.Experiments.CompileResults;

internal class AttemptGoto
{
    /*
     * This is an attempt to figure out what a compiled program could look like.
     * I'm hoping this design would support some of the more dynamic goto features of BASIC.
     * Note: This is just a sketch. I will not give it any real test yet.
     */
    public void ABLMain()
    {
    /* example program
     * 5 input n
     * 10 print n
     * 15 print n + 1
     * 20 print n + 5
     * 25 goto n
     */

    //Note: I'm using console here because of convenience.
    ABL_Label_5:
        var readValue = Console.ReadLine();
    ABL_Label_10:
        var readValueInt = int.Parse(readValue!);
        Console.WriteLine(readValueInt);
    ABL_Label_15:
        Console.WriteLine(readValueInt + 1);
    ABL_Label_20:
        Console.WriteLine(readValueInt + 5);
    ABL_Label_25:
        goto ABL_Label_GOTO_Switch;
    ABL_Label_GOTO_Switch:
        switch (readValueInt)
        {
            case 5:
                goto ABL_Label_5;
            case 10:
                goto ABL_Label_10;
            case 15:
                goto ABL_Label_15;
            case 20:
                goto ABL_Label_20;
            case 25:
                goto ABL_Label_25;
            default:
                throw new SystemException("invalid goto call");
        }
    }
}