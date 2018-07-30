using Common.Exceptions;
using Common.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sharp_LR35902_Compiler
{
	public class Lexer // Tokenizer
	{
		private static readonly Regex SplitRegex = new Regex(@"([a-z]+|\,|\(|\)|\=\=|\!\=|\=|\<|\>|\;|\+\+|\-\-|\{|\}|[0-9]|if|while|for|else+)", RegexOptions.Compiled | RegexOptions.Singleline);

		public static List<Token> CreateAST(string line) => CreateAST(new[] { line });
		public static List<Token> CreateAST(string[] lines)
		{
			var list = new List<Token>();

			foreach (var l in lines)
			{
				// Pre-Processing
				var line = l.Trim();

				var symbols = SplitRegex.Matches(line);
				foreach (Match symbol in symbols)
					list.Add(
						new Token(
							GetTokenType(symbol.Value),
							symbol.Value
						)
					);
			}

			return list;
		}

		public static TokenType GetTokenType(string value)
		{
			switch (value)
			{
				case "=":
				case "++":
				case "--":
					return TokenType.Operator;
				case "int":
					return TokenType.DataType;
				case "==":
				case "!=":
					return TokenType.Comparison;
				case "if":
				case "while":
				case "for":
				case "do":
				case "return":
				case "continue":
				case "else":
					return TokenType.ControlFlow;
				case "(":
				case ")":
				case "{":
				case "}":
				case ";":
					return TokenType.Grammar;
				default:
					if (value[0].IsLetter() || value[0].isNumber())
						return TokenType.Immediate;

					throw new SyntaxException("Unknown symbol");
			}
		}
	}
}
