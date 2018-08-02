using Common.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

using static Sharp_LR35902_Compiler.Parser;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Parser
	{
		[TestMethod]
		public void Increment()
		{
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "++")
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(1, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(IncrementNode));
			Assert.AreEqual(variablename, ((IncrementNode)(children[0])).VariableName);
		}

		[TestMethod]
		public void Decrement()
		{
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "--")
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(1, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(DecrementNode));
			Assert.AreEqual(variablename, ((DecrementNode)(children[0])).VariableName);
		}

		[TestMethod]
		public void VariableAssignment_WithImmediate()
		{
			var variablename = "x";
			var variablevalue = 42;
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(1, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(VariableAssignmentNode));
			Assert.AreEqual(variablename, (children[0] as VariableAssignmentNode).VariableName);
			Assert.IsInstanceOfType((children[0] as VariableAssignmentNode).Value, typeof(ImmediateValueNode));
			Assert.AreEqual(variablevalue, ((children[0] as VariableAssignmentNode).Value as ImmediateValueNode).Value);
		}

		[TestMethod]
		public void VariableAssignment_WithVariable()
		{
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Variable, othervariablename)
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(1, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(VariableAssignmentNode));
			Assert.AreEqual(variablename, (children[0] as VariableAssignmentNode).VariableName);
			Assert.IsInstanceOfType((children[0] as VariableAssignmentNode).Value, typeof(VariableValueNode));
			Assert.AreEqual(othervariablename, ((children[0] as VariableAssignmentNode).Value as VariableValueNode).Name);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void VariableAssignment_UnexpectedToken()
		{
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Grammar, ";")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void VariableAssignment_UnknownOperator()
		{
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "!="),
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void DeclareVariable()
		{
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";")
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(1, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.AreEqual(datatype, (children[0] as VariableDeclarationNode).DataType);
			Assert.AreEqual(variablename, (children[0] as VariableDeclarationNode).Name);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void DeclareVariable_VariableRedeclaration()
		{
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void DeclareVariable_CorrectNumberSymbols()
		{
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, ";")
			};

			var nodes = CreateAST(tokens).GetChildren();

			Assert.AreEqual(2, nodes.Length);
		}

		[TestMethod]
		public void DeclareAndAssignVariable_CorrectNumberSymbols()
		{
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, "42"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, ";")
			};

			var nodes = CreateAST(tokens).GetChildren();

			Assert.AreEqual(3, nodes.Length);
		}

		[TestMethod]
		public void DeclareAndAssignVariable_WithImmediate()
		{
			var datatype = "byte";
			var variablename = "x";
			var variablevalue = 42;
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(2, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.AreEqual(datatype, (children[0] as VariableDeclarationNode).DataType);
			Assert.AreEqual(variablename, (children[0] as VariableDeclarationNode).Name);
			Assert.IsInstanceOfType(children[1], typeof(VariableAssignmentNode));
			Assert.AreEqual(variablename, (children[1] as VariableAssignmentNode).VariableName);
			Assert.IsInstanceOfType((children[1] as VariableAssignmentNode).Value, typeof(ImmediateValueNode));
			Assert.AreEqual(variablevalue, ((children[1] as VariableAssignmentNode).Value as ImmediateValueNode).Value);
		}

		[TestMethod]
		public void DeclareAndAssignVariable_WithVariable()
		{
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Variable, othervariablename)
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(2, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.AreEqual(datatype, (children[0] as VariableDeclarationNode).DataType);
			Assert.AreEqual(variablename, (children[0] as VariableDeclarationNode).Name);
			Assert.IsInstanceOfType(children[1], typeof(VariableAssignmentNode));
			Assert.AreEqual(variablename, (children[1] as VariableAssignmentNode).VariableName);
			Assert.IsInstanceOfType((children[1] as VariableAssignmentNode).Value, typeof(VariableValueNode));
			Assert.AreEqual(othervariablename, ((children[1] as VariableAssignmentNode).Value as VariableValueNode).Name);
		}

		[TestMethod]
		public void DeclareAndAssignVariable_UnexpectedSymbol()
		{
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Variable, othervariablename)
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(2, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.AreEqual(datatype, (children[0] as VariableDeclarationNode).DataType);
			Assert.AreEqual(variablename, (children[0] as VariableDeclarationNode).Name);
			Assert.IsInstanceOfType(children[1], typeof(VariableAssignmentNode));
			Assert.AreEqual(variablename, (children[1] as VariableAssignmentNode).VariableName);
			Assert.IsInstanceOfType((children[1] as VariableAssignmentNode).Value, typeof(VariableValueNode));
			Assert.AreEqual(othervariablename, ((children[1] as VariableAssignmentNode).Value as VariableValueNode).Name);
		}
	}
}
