using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Sharp_LR35902_Compiler.Lexer;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Lexer
	{
		[TestMethod]
		public void GetTokenType_Assign()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("="));
		}

		[TestMethod]
		public void GetTokenType_Increment()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("++"));
		}

		[TestMethod]
		public void GetTokenType_AdditionAssignment()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("+="));
		}

		[TestMethod]
		public void GetTokenType_SubtractionAssignment()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("-="));
		}

		[TestMethod]
		public void GetTokenType_Decrement()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("--"));
		}

		[TestMethod]
		public void GetTokenType_Add()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("+"));
		}

		[TestMethod]
		public void GetTokenType_Subtract()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("-"));
		}


		[TestMethod]
		public void GetTokenType_Byte()
		{
			Assert.AreEqual(TokenType.DataType, GetTokenType("byte"));
		}

		[TestMethod]
		public void GetTokenType_Equals()
		{
			Assert.AreEqual(TokenType.Comparison, GetTokenType("=="));
		}

		[TestMethod]
		public void GetTokenType_NotEquals()
		{
			Assert.AreEqual(TokenType.Comparison, GetTokenType("!="));
		}

		[TestMethod]
		public void GetTokenType_If()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("if"));
		}

		[TestMethod]
		public void GetTokenType_While()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("while"));
		}

		[TestMethod]
		public void GetTokenType_For()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("for"));
		}

		[TestMethod]
		public void GetTokenType_Do()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("do"));
		}

		[TestMethod]
		public void GetTokenType_Return()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("return"));
		}

		[TestMethod]
		public void GetTokenType_Continue()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("continue"));
		}

		[TestMethod]
		public void GetTokenType_Else()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("else"));
		}

		[TestMethod]
		public void GetTokenType_OpenParentheses()
		{
			Assert.AreEqual(TokenType.Grammar, GetTokenType("("));
		}

		[TestMethod]
		public void GetTokenType_CloseParentheses()
		{
			Assert.AreEqual(TokenType.Grammar, GetTokenType(")"));
		}

		[TestMethod]
		public void GetTokenType_OpenBrace()
		{
			Assert.AreEqual(TokenType.Grammar, GetTokenType("{"));
		}

		[TestMethod]
		public void GetTokenType_CloseBrace()
		{
			Assert.AreEqual(TokenType.Grammar, GetTokenType("}"));
		}

		[TestMethod]
		public void GetTokenType_Semicolon()
		{
			Assert.AreEqual(TokenType.Grammar, GetTokenType(";"));
		}

		[TestMethod]
		public void GetTokenType_VariableName()
		{
			Assert.AreEqual(TokenType.Variable, GetTokenType("variable"));
		}

		[TestMethod]
		public void GetTokenType_Label()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("label:"));
		}

		[TestMethod]
		public void GetTokenType_Goto()
		{
			Assert.AreEqual(TokenType.ControlFlow, GetTokenType("goto"));
		}

		[TestMethod]
		public void GetTokenType_Value()
		{
			Assert.AreEqual(TokenType.Immediate, GetTokenType("42"));
		}

		[TestMethod]
		public void GetTokenType_Booleans()
		{
			Assert.AreEqual(TokenType.Immediate, GetTokenType("true"));
			Assert.AreEqual(TokenType.Immediate, GetTokenType("false"));
		}

		[TestMethod]
		public void GetTokenType_Negate()
		{
			Assert.AreEqual(TokenType.Operator, GetTokenType("!"));
		}

		[TestMethod]
		public void GetTokenType_And()
		{
			Assert.AreEqual(TokenType.Comparison, GetTokenType("&&"));
		}

		[TestMethod]
		public void GetTokenType_Or()
		{
			Assert.AreEqual(TokenType.Comparison, GetTokenType("||"));
		}

		[TestMethod]
		[ExpectedException(typeof(Common.Exceptions.SyntaxException))]
		public void GetTokenType_Unknown()
		{
			GetTokenList("@");
		}

		[TestMethod]
		public void GetTokenType_BlankLine()
		{
			var tokens = GetTokenList(" \t");
			Assert.AreEqual(0, tokens.Count);
		}

		private static TokenType GetTokenType(string token)
		{
			var tokens = GetTokenList(token);
			Assert.AreEqual(1, tokens.Count);

			return tokens[0].Type;
		}

		[TestMethod]
		public void CreateAST_IgnoreWhitespace()
		{
			var tokens = GetTokenList("	if	");

			Assert.AreEqual(1, tokens.Count);
		}

		[TestMethod]
		public void GetTokenList_VariableDecleration()
		{
			var decleration = "byte x = 0;";

			var tokens = GetTokenList(decleration);
			Assert.AreEqual(5, tokens.Count);
			Assert.AreEqual("byte", tokens[0].Value);
			Assert.AreEqual("x", tokens[1].Value);
			Assert.AreEqual("=", tokens[2].Value);
			Assert.AreEqual("0", tokens[3].Value);
			Assert.AreEqual(";", tokens[4].Value);
		}
	}
}
