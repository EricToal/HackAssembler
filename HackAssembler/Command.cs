using System;
using System.Text.RegularExpressions;
using System.Linq;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace HackAssembler
{
    public struct Command
    {
        public string Type { get; }
        public string Instruction { get; }
        public string Symbol { get; }

        private Command(string type, string instruction, 
            string symbol = "Xxx")
        {
            Type = type;
            Instruction = instruction;
            Symbol = symbol;
        }

        public static Exceptional<Command> Create(string command, SymbolTable table)
        {
            string symbol;
            try
            {
                switch (true)
                {
                    case bool _ when Regex.IsMatch(command, "[AMD]+[=]{1}[AMD01+!&|-]+"):
                        return new Command("C_COMMAND", CommandExt.CreateCInstruction(
                            ComputationCode.Create(String.Join("", command
                                                    .SkipWhile(x => x != '=')
                                                    .Skip(1)
                                                    .TakeWhile(x => x != ' ' && x != '/'))),
                            DestinationCode.Create(String.Join("", command
                                                    .TakeWhile(x => x != '='))),
                            JumpCode.Create("000")));

                    case bool _ when Regex.IsMatch(command, "[D0]{1};{1}[JGTEQLNMP]+"):
                        return new Command("C_COMMAND", CommandExt.CreateCInstruction(
                            ComputationCode.Create(String.Join("", command
                                                    .SkipWhile(x => x == ' ')
                                                    .TakeWhile(x => x != ';'))),
                            DestinationCode.Create("000"),
                            JumpCode.Create(String.Join("", command
                                              .SkipWhile(x => x != ';')
                                              .Skip(1)
                                              .TakeWhile(x => x != ' ' && x != '/')))));

                    case bool _ when Regex.IsMatch(command, "@{1}[0-9]+"):
                        symbol = String.Join("", command
                            .SkipWhile(x => x == ' ' || x == '@')
                            .TakeWhile(x => x != ' ' && x != '/'));
                        return new Command("A_COMMAND",
                            CommandExt.CreateAInstructionNoSymbols(symbol),
                            symbol);

                    case bool _ when Regex.IsMatch
                    (command, "[@]{1}[A-Za-z_$:.]{1}[A-Za-z0-9_$:.]+"):
                        symbol = String.Join("", command
                            .SkipWhile(x => x == ' ' || x == '@')
                            .TakeWhile(x => x != ' ' && x != '/'));
                        return new Command("A_COMMAND",
                            CommandExt.CreateAInstructionFromSymbol(symbol, table),
                            symbol);

                    default:
                        throw new ArgumentException
                            ($"Command '{command}' is not translatable");
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

    public static class CommandExt
    {
        public static string CreateCInstruction
            (ComputationCode comp, DestinationCode dest, JumpCode jump)
            => "111" + comp.Code + dest.Code + jump.Code;

        public static string CreateAInstructionNoSymbols(string address)
            => Parse(address).Match(
                None: () => throw new ArgumentException($"Address {address} is Invalid"),
                Some: (some) => Convert
                                    .ToString(some, 2)
                                    .PrependChar('0'));

        public static string CreateAInstructionFromSymbol
            (string symbol, SymbolTable table)
        {
            if (!table.ContainsEntry(symbol))
                table.AddVariable(symbol);

            return CreateAInstructionNoSymbols(table.GetAddressFromEntry(symbol));                
        }
        private static Option<int> Parse(string str)
        {
            int parsed;
            if (Int32.TryParse(str, out parsed))
                return parsed;
            else
                return None;
        }

        private static string PrependChar
            (this string @this, char ch)
        {
            int length = 16 - @this.Length;
            string temp = "";

            foreach (int i in Range(1, length))
            {
                temp = temp + "0";
            }
            return temp + @this;
        }
    }

    public struct ComputationCode
    {
        public string Code { get; }

        private ComputationCode(string comp)
        {
            Code = comp;
        }

        public static ComputationCode Create(string comp)
            => comp switch
            {
                "0"   =>        new ComputationCode("0101010"),
                "1"   =>        new ComputationCode("0111111"),
                "-1"  =>        new ComputationCode("0111010"),
                "D"   =>        new ComputationCode("0001100"),
                "A"   =>        new ComputationCode("0110000"),
                "M"   =>        new ComputationCode("1110000"),
                "!D"  =>        new ComputationCode("0001111"),
                "!A"  =>        new ComputationCode("0110001"),
                "!M"  =>        new ComputationCode("1110001"),
                "-D"  =>        new ComputationCode("0001111"),
                "-A"  =>        new ComputationCode("0110011"),
                "-M"  =>        new ComputationCode("1110011"),
                "D+1" =>        new ComputationCode("0011111"),
                "A+1" =>        new ComputationCode("0110111"),
                "M+1" =>        new ComputationCode("1110111"),
                "D-1" =>        new ComputationCode("0001110"),
                "A-1" =>        new ComputationCode("0110010"),
                "M-1" =>        new ComputationCode("1110010"),
                "D+A" =>        new ComputationCode("0000010"),
                "D+M" =>        new ComputationCode("1000010"),
                "D-A" =>        new ComputationCode("0010011"),
                "D-M" =>        new ComputationCode("1010011"),
                "A-D" =>        new ComputationCode("0000111"),
                "M-D" =>        new ComputationCode("1000111"),
                "D&A" =>        new ComputationCode("0000000"),
                "D&M" =>        new ComputationCode("1000000"),
                "D|A" =>        new ComputationCode("0010101"),
                "D|M" =>        new ComputationCode("1010101"),
                _     =>        throw new ArgumentException($"Computation Code {comp} is Invalid")
            };
    }

    public struct DestinationCode
    {
        public string Code { get; }

        private DestinationCode(string dest)
        {
            Code = dest;
        }

        public static DestinationCode Create(string dest)
            => dest switch
            {
                "M"   =>        new DestinationCode("001"),
                "D"   =>        new DestinationCode("010"),
                "MD"  =>        new DestinationCode("011"),
                "A"   =>        new DestinationCode("100"),
                "AM"  =>        new DestinationCode("101"),
                "AD"  =>        new DestinationCode("110"),
                "AMD" =>        new DestinationCode("111"),
                "000" =>        new DestinationCode("000"),
                _     =>        throw new 
                    ArgumentException("Destination Code {dest} is Invalid.")
            };
    }

    public struct JumpCode
    {
        public string Code { get; }

        private JumpCode(string jump)
        {
            Code = jump;
        }

        public static JumpCode Create(string jump)
            => jump switch
            {
                "JGT" =>        new JumpCode("001"),
                "JEQ" =>        new JumpCode("010"),
                "JGE" =>        new JumpCode("011"),
                "JLT" =>        new JumpCode("100"),
                "JNE" =>        new JumpCode("101"),
                "JLE" =>        new JumpCode("110"),
                "JMP" =>        new JumpCode("111"),
                "000" =>        new JumpCode("000"),
                _     =>        throw new
                    ArgumentException("Jump Code {jump} is Invalid")
            };
    }
}
