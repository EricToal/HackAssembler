using System;

namespace HackAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("Usage: HackAssembler filename.asm\n");
            else
                AssemblyController.Translate(args[0]);
        }
    }
}
