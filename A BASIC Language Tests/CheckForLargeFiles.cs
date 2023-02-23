using System.Collections.Generic;
using FileUtility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace A_BASIC_Language_Tests;

[TestClass]
public class CheckForLargeFiles
{
    [TestMethod]
    public void CountSourceFileLines()
    {
        var d = new PathInfo(new FileInfo(@"../../../../"));
        var files = d.GetDirectoryContent(10);
        var sourceFiles = new List<SourceFile>();
        foreach (var f in files.Where(x => x.FullName.ToLower().EndsWith(".cs")))
        {
            if (!f.ContainsFile)
                continue;

            var contentStream = f.FileInfo!.OpenText();
            var content = contentStream.ReadToEnd();
            var rowCount = Regex.Split(content, "\r\n|\r|\n").Length;
            sourceFiles.Add(new SourceFile(f.FileInfo.Name, rowCount));
        }
    }

    private class SourceFile
    {
        public string FileName { get; }
        public int Rows { get; }

        public SourceFile(string fileName, int rows)
        {
            FileName = fileName;
            Rows = rows;
        }
    }
}