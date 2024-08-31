using System.Collections.Generic;
using System.Linq;
using Xunit;
using LaYumba.Functional;

namespace HackAssembler.Tests
{
    public class ParserTest
    {
        [Fact]
        public static void CreateCommandsListTest()
        {
            var commands = new List<string> { "M=1", "M=0", "D=M", "D=D-A",
            "M=D+M", "M=M+1", "D;JGT", "0;JMP", "D;JLE" };
            var parsedCommands = ParsedCommandsExt.CreateCommandsList(commands);
            var binaryCommands = commands.Select
                (x => Command.Create(x, new SymbolTable())).ToList();

            Assert.Equal<Exceptional<Command>>(parsedCommands, binaryCommands);
        }
    }
}
