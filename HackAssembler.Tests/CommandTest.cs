using System;
using Xunit;

namespace HackAssembler.Tests
{
    public class CommandTest
    {
        [Theory]
        [InlineData("M=1", "1110111111001000")]
        [InlineData("M=0", "1110101010001000")]
        [InlineData("D=M", "1111110000010000")]
        [InlineData("D=D-A", "1110010011010000")]
        [InlineData("M=D+M", "1111000010001000")]
        [InlineData("M=M+1", "1111110111001000")]
        [InlineData("D;JGT", "1110001100000001")]
        [InlineData("0;JMP", "1110101010000111")]
        [InlineData("D;JLE", "1110001100000110")]
        [InlineData("invalid", "any")]
        [InlineData("M=-1", "1110111010001000")]
        public static void CreateCCommandTest(string src, string rslt)
        {
            var command = Command.Create(src, new SymbolTable());
            command.Match(
                error   =>  Assert.NotNull(error),
                success =>  Assert.Equal(rslt, success.Instruction)); 
        }

        [Theory]
        [InlineData("@100", "0000000001100100")]
        [InlineData("@0", "0000000000000000")]
        [InlineData("@32", "0000000000100000")]
        [InlineData("@-1", "any")]
        [InlineData("invalid", "any")]
        [InlineData("@", "any")]
        public static void CreateACommandNoSymbolsTest(string src, string rslt)
        {
            var command = Command.Create(src, new SymbolTable());
            command.Match(
                error   =>  Assert.NotNull(error),
                success =>  Assert.Equal(rslt, success.Instruction));
        }
    }

    public class ComputationCodeTest
    {
        [Theory]
        [InlineData("0", "0101010")]
        [InlineData("1", "0111111")]
        [InlineData("-1", "0111010")]
        [InlineData("D", "0001100")]
        [InlineData("A", "0110000")]
        [InlineData("M", "1110000")]
        [InlineData("!D", "0001111")]
        [InlineData("!A", "0110001")]
        [InlineData("!M", "1110001")]
        [InlineData("-D", "0001111")]
        [InlineData("-A", "0110011")]
        [InlineData("-M", "1110011")]
        [InlineData("D+1", "0011111")]
        [InlineData("A+1", "0110111")]
        [InlineData("M+1", "1110111")]
        [InlineData("D-1", "0001110")]
        [InlineData("A-1", "0110010")]
        [InlineData("M-1", "1110010")]
        [InlineData("D+A", "0000010")]
        [InlineData("D+M", "1000010")]
        [InlineData("D-A", "0010011")]
        [InlineData("D-M", "1010011")]
        [InlineData("A-D", "0000111")]
        [InlineData("M-D", "1000111")]
        [InlineData("D&A", "0000000")]
        [InlineData("D&M", "1000000")]
        [InlineData("D|A", "0010101")]
        [InlineData("D|M", "1010101")]
        public void CreateCompCodeTest(string src, string rslt)
        {
            var compCode = ComputationCode.Create(src);
            Assert.Equal(compCode.Code, rslt);
        }
        
        [Fact]
        public void CreateCompCodeInvalidTest()
        => Assert.Throws<ArgumentException>(() => ComputationCode.Create("lol"));
    }

    public class DestinationCodeTest
    {
        [Theory]
        [InlineData("M", "001")]
        [InlineData("D", "010")]
        [InlineData("MD", "011")]
        [InlineData("A", "100")]
        [InlineData("AM", "101")]
        [InlineData("AD", "110")]
        [InlineData("AMD", "111")]
        [InlineData("000", "000")]
        public void CreateDestCodeTest(string src, string rslt)
        {
            var destCode = DestinationCode.Create(src);
            Assert.Equal(destCode.Code, rslt);
        }

        [Fact]
        public void CreateDestCodeInvalidTest()
        => Assert.Throws<ArgumentException>(() => DestinationCode.Create("lol"));
    }

    public class JumpCodeTest
    {
        [Theory]
        [InlineData("JGT", "001")]
        [InlineData("JEQ", "010")]
        [InlineData("JGE", "011")]
        [InlineData("JLT", "100")]
        [InlineData("JNE", "101")]
        [InlineData("JLE", "110")]
        [InlineData("JMP", "111")]
        [InlineData("000", "000")]
        public void CreateJumpCodeTest(string src, string rslt)
        {
            var jumpCode = JumpCode.Create(src);
            Assert.Equal(jumpCode.Code, rslt);
        }

        [Fact]
        public void CreateJumpCodeInvalidTest()
        => Assert.Throws<ArgumentException>(() => JumpCode.Create("lol"));
    }
}
