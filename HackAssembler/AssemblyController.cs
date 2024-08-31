using System;

namespace HackAssembler
{
    class AssemblyController
    {
        public static ValueTuple Translate(string fileName)
        => FileReader.ReadFile(fileName)
            .Match(
                (fileNotRead) => PrintFailureToConsole(fileNotRead),
                (fileRead) => FileWriter
                    .WriteHackAndLogFiles(new ParsedCommands
                        (fileRead), fileName)
                        .Match(
                            (fileWriteFailure) => PrintFailureToConsole(fileWriteFailure),
                            (fileWriteSuccess) => new ValueTuple()));

        private static ValueTuple PrintFailureToConsole(Exception ex)
        {
            Console.WriteLine(ex.Message + "\n" +
                    ex.StackTrace + "\n--------------------\n\n");
            return new ValueTuple();
        }
    }
}
