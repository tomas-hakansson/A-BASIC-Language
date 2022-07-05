﻿using System.Text;

namespace A_BASIC_Language.IO;

public class FileIO : IOBase
{
    public FileIO(string filename) : base(filename)
    {
    }

    public override LoadResult Load()
    {
        try
        {
            using var x = new StreamReader(Filename, Encoding.UTF8);
            var source = x.ReadToEnd();
            x.Close();
            return LoadResult.Success(source);
        }
        catch
        {
            return LoadResult.Fail();
        }
    }
}