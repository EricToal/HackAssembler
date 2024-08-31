using System;
using System.Linq;
using System.IO;
using LaYumba.Functional;

namespace HackAssembler
{
    public static class FileWriter
    {
        public static Exceptional<ValueTuple> WriteHackAndLogFiles
            (ParsedCommands parsed, string fileName)
        {
            try
            {
                FileStream hackFile = File.Create(
                    FileWriter.GetHackFilePath(fileName));
                FileStream logFile = File.Create(
                     FileWriter.GetLogFilePath(fileName));

                using (StreamWriter logSW = new StreamWriter(logFile))
                using (StreamWriter hackSW = new StreamWriter(hackFile))
                {
                    parsed.Commands.ForEach(x => x.Match(
                        (failure) => logSW.Write
                            (failure.Message + "\n" + failure.StackTrace +
                            "\n\n----------------------\n\n"),
                        (success) => hackSW.WriteLine
                            (success.Instruction)));
                }


                return new ValueTuple();
            }
            catch (Exception ex)
            {
                return ex;
            }

        }

        private static string GetFileNameNoExt(string fileName)
            => String.Join("", fileName.TakeWhile(x => x != '.'));

        private static string GetFilePath(string dir, string fileName, string ext)
            => dir + fileName + ext;

        private static string GetWorkingDirectory()
            => AppDomain.CurrentDomain.BaseDirectory;

        private static string GetHackFilePath(string fileName)
            => GetFilePath(GetWorkingDirectory(), GetFileNameNoExt(fileName),
                ".hack");

        private static string GetLogFilePath(string fileName)
            => GetFilePath(GetWorkingDirectory(), GetFileNameNoExt(fileName),
                ".log");
    }
}
