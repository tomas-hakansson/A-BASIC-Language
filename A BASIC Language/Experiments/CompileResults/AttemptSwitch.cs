namespace A_BASIC_Language.Experiments.CompileResults;
internal class AttemptSwitch
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
        string? N_Label_5 = String.Empty;
        float N_int_Label_5 = 0;
        int goto_value = 5;
        while (true)
        {
            switch (goto_value)
            {
                case 5:
                    N_Label_5 = Console.ReadLine();
                    N_int_Label_5 = int.Parse(N_Label_5!);
                    goto case 10;
                case 10:
                    Console.WriteLine(N_int_Label_5);
                    goto case 15;
                case 15:
                    Console.WriteLine(N_int_Label_5 + 1);
                    goto case 20;
                case 20:
                    Console.WriteLine(N_int_Label_5 + 5);
                    goto case 25;
                case 25:
                    goto_value = (int)N_int_Label_5;
                    continue;
                default:
                    throw new SystemException("Invalid goto call");
            }
        }
    }
}