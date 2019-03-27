using System;
using System.IO;

namespace Sharp_LR35902_Compiler {
	public class Program {
		public static void Main(string[] args) {
			var lines = File.ReadAllLines(args[0]);
			var tokens = Lexer.GetTokenList(lines);
			var ast = Parser.CreateAST(tokens);
			Optimizer.Optimize(ast);
			Optimizer.Simplify(ast);
			foreach (var line in Compiler.EmitAssembly(ast))
				Console.WriteLine(line);
		}
	}
}
