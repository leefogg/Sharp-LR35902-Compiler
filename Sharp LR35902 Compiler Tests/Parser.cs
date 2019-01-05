﻿using Common.Exceptions;
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
		public void CreateAST_Increment()
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
		public void CreateAST_Decrement()
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
		public void CreateAST_VariableAssignment_WithImmediate()
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
		public void CreateAST_VariableAssignment_WithVariable()
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
			Assert.AreEqual(othervariablename, ((children[0] as VariableAssignmentNode).Value as VariableValueNode).VariableName);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_UnexpectedToken()
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
		public void CreateAST_VariableAssignment_UnknownOperator()
		{
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "!="),
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void CreateAST_DeclareVariable()
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
			Assert.AreEqual(variablename, (children[0] as VariableDeclarationNode).VariableName);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareVariable_VariableRedeclaration()
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
		public void CreateAST_DeclareVariable_CorrectNumberSymbols()
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
		public void CreateAST_DeclareAndAssignVariable_CorrectNumberSymbols()
		{
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, "42"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, ";")
			};

			var nodes = CreateAST(tokens).GetChildren();

			Assert.AreEqual(3, nodes.Length);
		}

		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_WithImmediate()
		{
			var datatype = "byte";
			var variablename = "x";
			byte variablevalue = 42;
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ImmediateValueNode(variablevalue)));

			Assert.IsTrue(compareNode(expectedAST, ast));
		}

		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_WithVariable()
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

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new VariableValueNode(othervariablename)));

			Assert.IsTrue(compareNode(expectedAST, ast));
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_ExpectedEquals()
		{
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "-"), // oops, programmer typo
				new Token(TokenType.Variable, othervariablename)
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_UnexpectedSymbol()
		{
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Grammar, ")")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void CreateAST_Label()
		{
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "label:")
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual(1, children.Length);
			Assert.IsInstanceOfType(children[0], typeof(LabelNode));
		}

		private bool compareNode(Node expected, Node actual)
		{
			var actualchildren = actual.GetChildren();
			var expectedchilren = expected.GetChildren();

			if (actualchildren.Length != expectedchilren.Length)
				return false;

			for (var i = 0; i < expectedchilren.Length; i++)
				if (!actualchildren[i].Equals(expectedchilren[i]))
					return false;

			for (var i = 0; i < actualchildren.Length; i++)
				return compareNode(expectedchilren[i], actualchildren[i]);

			return true; // Only if both have no children
		}
	}
}
