using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Linq;

namespace HackAssembler
{
    public class SymbolTable
    {
        public ImmutableDictionary<String, String> Table { get; set; }
        public int AddressCounter { get; set; }

        public SymbolTable(int addressCounter = 16)
        {
            Table = new Dictionary<string, string>
            {
                { "SP", "0" },
                { "LCL", "1" },
                { "ARG", "2" },
                { "THIS", "3" },
                { "THAT", "4" },
                { "SCREEN", "16384" },
                { "KBD", "24576" },
                { "R0", "0" }, { "R1", "1" }, { "R2", "2" }, { "R3", "3" },
                { "R4", "4" }, { "R5", "5" }, { "R6", "6" }, { "R7", "7" },
                { "R8", "8" }, { "R9", "9" }, { "R10", "10" }, { "R11", "11" },
                { "R12", "12" }, { "R13", "13" }, { "R14", "14" }, { "R15", "15" }
            }.ToImmutableDictionary<string, string>();

            AddressCounter = addressCounter;
        }

        public SymbolTable(Dictionary<string, string> table, int addressCounter = 16)
        {
            Table = table.ToImmutableDictionary<string, string>();
            AddressCounter = addressCounter;
        }
    }

    public static class SymbolTableExt
    {
        public static ValueTuple AddLabel
            (this SymbolTable @this, string symbol, int counter)
        {
            @this.Table = @this.Table.Add(symbol, counter.ToString());
            return new ValueTuple();
        }
        public static ValueTuple AddVariable
            (this SymbolTable @this, string symbol)
        {
            @this.Table = @this.Table.Add(symbol, @this.AddressCounter.ToString());
            @this.AddressCounter++;
            return new ValueTuple();
        }

        public static bool ContainsEntry(this SymbolTable @this, string symbol)
            => @this.Table.ContainsKey(symbol);

        public static string GetAddressFromEntry
            (this SymbolTable @this, string symbol)
            => @this.Table[symbol];

        public static SymbolTable StoreLabelsInTable
            (this SymbolTable @this, IEnumerable<String> instructions)
        {
            int instructionCounter = 0;
            string symbol;

            foreach (String str in instructions)
            {
                switch (true)
                {
                    case bool _ when Regex
                        .IsMatch(str, "[(]{1}[A-Za-z_$:.]{1}[A-Za-z0-9_$:.]+[)]{1}"):

                        symbol = String.Join("", str
                            .SkipWhile(x => x != '(')
                            .Skip(1)
                            .TakeWhile(x => x != ')'));
                        if (!@this.ContainsEntry(symbol))
                            @this.AddLabel(symbol, instructionCounter);
                        break;

                    case bool _ when Regex
                        .IsMatch(str, "[@]{1}[A-Za-z_$:.]{1}[A-Za-z0-9_$:.]+"):

                        instructionCounter++;
                        break;

                    case bool _ when Regex
                        .IsMatch(str, "[@]{1}[0-9]+"):

                        instructionCounter++;
                        break;
                    case bool _ when Regex
                        .IsMatch(str, "[AMD]+[=]{1}[AMD01+!&|-]+"):

                        instructionCounter++;
                        break;

                    case bool _ when Regex
                        .IsMatch(str, "[D0]{1};{1}[JGTEQLNMP]+"):

                        instructionCounter++;
                        break;

                    default:
                        break;
                }
            }
            return @this;
        }
    }
}
