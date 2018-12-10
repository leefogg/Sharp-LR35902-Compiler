﻿using Common.Exceptions;
using Common.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sharp_LR35902_Compiler
{
	public class Lexer // Tokenizer
	{
		private struct TokenDescriptor
		{
			public readonly Regex Pattern;
			public readonly TokenType Type;

			public TokenDescriptor(string pattern, TokenType type)
			{
				Pattern = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
				Type = type;
			}
		}

		private static readonly TokenDescriptor[] PossibleTokens = {
			new TokenDescriptor("while",	TokenType.ControlFlow),
			new TokenDescriptor("do",		TokenType.ControlFlow),
			new TokenDescriptor("for",		TokenType.ControlFlow),
			new TokenDescriptor("return",	TokenType.ControlFlow),
			new TokenDescriptor("continue", TokenType.ControlFlow),
			new TokenDescriptor("else",		TokenType.ControlFlow),
			new TokenDescriptor("if",		TokenType.ControlFlow),
			new TokenDescriptor("int",		TokenType.DataType),
			new TokenDescriptor(@"\=\=",	TokenType.Comparison),
			new TokenDescriptor(@"\!\=",	TokenType.Comparison),
			new TokenDescriptor(@"\+\+",	TokenType.Operator),
			new TokenDescriptor(@"\-\-",	TokenType.Operator),
			new TokenDescriptor(@"\=",		TokenType.Operator),
			new TokenDescriptor(@"\(",		TokenType.Grammar),
			new TokenDescriptor(@"\)",		TokenType.Grammar),
			new TokenDescriptor(@"\{",		TokenType.Grammar),
			new TokenDescriptor(@"\}",		TokenType.Grammar),
			new TokenDescriptor(@"\;",		TokenType.Grammar),
			new TokenDescriptor("[a-z]+",	TokenType.Variable),
			new TokenDescriptor("[0-9]+",	TokenType.Immediate),
		};

		public static List<Token> GetTokenList(string line) => GetTokenList(new[] { line });
		public static List<Token> GetTokenList(string[] lines)
		{
			var tokens = new List<Token>();

			for (int i=0; i<lines.Length; i++)
			{
				var line = lines[i].Trim();
				if (line.Length == 0)
					continue;

				var starttokencount = tokens.Count;

				foreach (var descriptor in PossibleTokens)
				{
					var symbols = descriptor.Pattern.Matches(line);
					foreach (Match symbol in symbols)
						tokens.Add(
							new Token(
								descriptor.Type,
								symbol.Value
							)
						);
					if (symbols.Count > 0)
						break;
				}

				if (tokens.Count == starttokencount) // Didn't add any tokens, didn't understand line
					throw new SyntaxException($"Unknown character on line {i+1}");
			}

			return tokens;
		}
	}
}
