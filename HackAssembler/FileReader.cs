using System;
using System.Collections.Generic;
using System.IO;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace HackAssembler
{
    public static class FileReader
    {
        public static Exceptional<IEnumerable<String>> ReadFile(string fileName)
        {
            string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = workingDirectory + fileName;

            try
            { return Exceptional(File.ReadLines(path)); }
            catch (Exception ex)
            { return ex; }
        }
    }
}
