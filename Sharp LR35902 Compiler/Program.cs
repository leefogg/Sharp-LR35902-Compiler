using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sharp_LR35902_Compiler
{
    public class Program
    {
		public static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);
			var tokens = Lexer.GetTokenList(lines);
			var ast = Parser.CreateAST(tokens);
			foreach (var line in Compiler.EmitAssembly(ast))
				Console.WriteLine(line);
		}
    }
}
