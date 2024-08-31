using System.Collections.Generic;
using System.Text.RegularExpressions;
using LaYumba.Functional;

namespace HackAssembler
{
    public struct ParsedCommands
    {
        public List<Exceptional<Command>> Commands { get; }

        public ParsedCommands(IEnumerable<string> lines)
        {
            Commands = ParsedCommandsExt.CreateCommandsList(lines);
        }
    }

    public static class ParsedCommandsExt
    {

        public static List<Exceptional<Command>> CreateCommandsList
            (IEnumerable<string> lines)
        {
            var commandsList = new List<Exceptional<Command>>();
            var symbolTable = new SymbolTable().StoreLabelsInTable(lines);
            foreach (string str in lines)
            {
                switch (true)
                {
                    case bool _ when Regex.IsMatch(str, "^[/]{2}"):
                        break;
                    case bool _ when Regex.IsMatch(str, "^(?![\\s\\S])"):
                        break;
                    case bool _ when Regex.IsMatch
                    (str, "[(]{1}[A-Za-z_$:.]{1}[A-Za-z0-9_$:.]+[)]{1}"):
                        break;
                    default:
                        commandsList.Add(Command.Create(str, symbolTable));
                        break;
                }
            }
            return commandsList;
        }
    }
}
