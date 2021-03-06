﻿using System.Collections.Generic;
using System.Linq;
using Common.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using Sharp_LR35902_Compiler.Nodes;
using Sharp_LR35902_Compiler.Nodes.Assignment;
using static Sharp_LR35902_Compiler.Parser;

namespace Sharp_LR35902_Compiler_Tests {
	[TestClass]
	public class Parser {
		[TestMethod]
		public void CreateAST_Increment() {
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Increment)
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren().ToList();

			Assert.AreEqual(1, children.Count);
			Assert.IsInstanceOfType(children[0], typeof(IncrementNode));
			Assert.AreEqual(variablename, ((IncrementNode)children[0]).VariableName);
		}

		[TestMethod]
		public void CreateAST_Decrement() {
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Decrement)
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren().ToList();

			Assert.AreEqual(1, children.Count);
			Assert.IsInstanceOfType(children[0], typeof(DecrementNode));
			Assert.AreEqual(variablename, ((DecrementNode)children[0]).VariableName);
		}

		[TestMethod]
		public void CreateAST_VariableAssignment_WithImmediate() {
			var variablename = "x";
			var datatype = "byte";
			byte variablevalue = 42;
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ShortValueNode(0)));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ShortValueNode(variablevalue)));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_MissingValueThrows()
		{
			var variablename = "x";
			var datatype = "byte";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Grammar, ";")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_VariableExists() {
			var variablename = "x";
			var variablevalue = 42;
			var tokens = new List<Token> {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void CreateAST_VariableAssignment_WithVariable() {
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, othervariablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Variable, othervariablename)
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ShortValueNode(0)));
			expectedAST.AddChild(new VariableDeclarationNode(datatype, othervariablename));
			expectedAST.AddChild(new VariableAssignmentNode(othervariablename, new ShortValueNode(0)));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new VariableValueNode(othervariablename)));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_WithVariable_ValueDoesntExist() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Variable, "y")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_WithVariable_AssignedDoesntExist() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Variable, "y")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_UnexpectedToken() {
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Grammar, ";")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_UnknownOperator() {
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "!=")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void CreateAST_DeclareVariable() {
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ShortValueNode(0)));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareVariable_DataTypeDoesntExists() {
			var datatype = "blah";
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareVariable_VariableRedeclaration() {
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token> {
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
		public void CreateAST_DeclareVariable_CorrectNumberSymbols() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, ";")
			};

			var nodes = CreateAST(tokens).GetChildren();

			Assert.AreEqual(4, nodes.Count());
		}

		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_CorrectNumberSymbols() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, "42"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, ";")
			};

			var nodes = CreateAST(tokens).GetChildren();

			Assert.AreEqual(4, nodes.Count());
		}

		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_WithImmediate() {
			var datatype = "byte";
			var variablename = "x";
			byte variablevalue = 42;
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ShortValueNode(variablevalue)));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_WithExpression() {
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, "5"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "5"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, "5")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new AdditionNode(new ShortValueNode(5), new ShortValueNode(5))));
			expectedAST.AddChild(new VariableDeclarationNode(datatype, "y"));
			expectedAST.AddChild(new VariableAssignmentNode("y", new ShortValueNode(5)));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_AdditionAssignment_WithVariable() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, "5"),
				new Token(TokenType.Grammar, ";"),

				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "+="),
				new Token(TokenType.Variable, "y")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedAST.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			expectedAST.AddChild(new VariableDeclarationNode("byte", "y"));
			expectedAST.AddChild(new VariableAssignmentNode("y", new ShortValueNode(5)));
			expectedAST.AddChild(new AdditionAssignmentNode("x", new VariableValueNode("y")));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_AdditionAssignment_WithImmediate() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),

				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "+="),
				new Token(TokenType.Immediate, "10")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedAST.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			expectedAST.AddChild(new AdditionAssignmentNode("x", new ShortValueNode(10)));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_SubtractionAssignment_WithVariable() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, "5"),
				new Token(TokenType.Grammar, ";"),

				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "-="),
				new Token(TokenType.Variable, "y")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedAST.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			expectedAST.AddChild(new VariableDeclarationNode("byte", "y"));
			expectedAST.AddChild(new VariableAssignmentNode("y", new ShortValueNode(5)));
			expectedAST.AddChild(new SubtractionAssignmentNode("x", new VariableValueNode("y")));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_SubtractionAssignment_WithImmediate() {
			var tokens = new List<Token> {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),

				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "-="),
				new Token(TokenType.Immediate, "10")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedAST.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			expectedAST.AddChild(new SubtractionAssignmentNode("x", new ShortValueNode(10)));

			CompareNode(expectedAST, ast);
		}


		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_WithVariable() {
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, othervariablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Variable, othervariablename)
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, othervariablename));
			expectedAST.AddChild(new VariableAssignmentNode(othervariablename, new ShortValueNode(0)));
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new VariableValueNode(othervariablename)));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_WithSelf_Throws()
		{
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Variable, variablename)
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_WithSelf_InExpression_Throws()
		{
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Immediate, "5"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Variable, variablename)
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_WithVariableExpression() {
			var datatype = "byte";
			var variablename = "x";
			var variablename1 = "y";
			var variablename2 = "z";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename1),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename2),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Variable, variablename1),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Variable, variablename2)
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename1));
			expectedAST.AddChild(new VariableAssignmentNode(variablename1, new ShortValueNode(0)));
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename2));
			expectedAST.AddChild(new VariableAssignmentNode(variablename2, new ShortValueNode(0)));
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new AdditionNode(new VariableValueNode(variablename1), new VariableValueNode(variablename2))));

			CompareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_WithVariable_DoesntExist() {
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Variable, othervariablename)
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_ExpectedEquals() {
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Subtract), // oops, programmer typo
				new Token(TokenType.Variable, othervariablename)
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_UnexpectedSymbol() {
			var datatype = "byte";
			var variablename = "x";
			var tokens = new List<Token> {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, BuiltIn.Operators.Assign),
				new Token(TokenType.Grammar, ")")
			};

			CreateAST(tokens);
		}

		// TODO: Test validating expression's return type being assigned to incompatible type variable

		[TestMethod]
		public void CreateAST_Label() {
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "label:")
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren().ToList();

			Assert.AreEqual(1, children.Count);
			Assert.IsInstanceOfType(children[0], typeof(LabelNode));
		}

		[TestMethod]
		public void CreateAST_Label_CropsName() {
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "label:")
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren().ToList();

			Assert.AreEqual(1, children.Count);
			Assert.AreEqual("label", (children[0] as LabelNode).Name);
		}

		[TestMethod]
		public void CreateAST_Goto() {
			var labelname = "label";
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "goto"),
				new Token(TokenType.Variable, labelname)
			};

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new GotoNode(labelname));

			CompareNode(expectedast, ast);
		}

		[TestMethod]
		public void CreateAST_IfStatement_CreatesBody()
		{
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "if"), 
				new Token(TokenType.Grammar, "("), 
				new Token(TokenType.Immediate, "1"), 
				new Token(TokenType.Grammar, ")"), 
				new Token(TokenType.Grammar, "{"), 
				new Token(TokenType.Grammar, "}"), 
			};

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new IfNode(new ShortValueNode(1), new BlockNode()));

			CompareNode(expectedast, ast);
		}

		[TestMethod]
		public void CreateAST_IfStatement_CapturesTrueBody()
		{
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "if"),
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),
				new Token(TokenType.Grammar, "{"),
				new Token(TokenType.DataType, "byte"),  
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, "="),
				new Token(TokenType.Immediate, "10"),
				new Token(TokenType.Grammar, "}"),
			};

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			var block = new BlockNode();
			block.AddChild(new VariableDeclarationNode("byte", "x"));
			block.AddChild(new VariableAssignmentNode("x", new ShortValueNode(10)));
			expectedast.AddChild(new IfNode(new ShortValueNode(1), block));

			CompareNode(expectedast, ast);
		}

		[TestMethod]
		public void CreateAST_IfStatement_CapturesFalseBody()
		{
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "if"),
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),
				new Token(TokenType.Grammar, "{"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, "="),
				new Token(TokenType.Immediate, "10"),
				new Token(TokenType.Grammar, ";"), 
				new Token(TokenType.Grammar, "}"),
				new Token(TokenType.Grammar, "else"), 
				new Token(TokenType.Grammar, "{"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, "="),
				new Token(TokenType.Immediate, "5"),
				new Token(TokenType.Grammar, ";"), 
				new Token(TokenType.Grammar, "}"),
			};

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			var iftrue = new BlockNode();
			iftrue.AddChild(new VariableDeclarationNode("byte", "x"));
			iftrue.AddChild(new VariableAssignmentNode("x", new ShortValueNode(10)));
			var iffalse = new BlockNode();
			iffalse.AddChild(new VariableDeclarationNode("byte", "x"));
			iffalse.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5)));
			expectedast.AddChild(new IfNode(new ShortValueNode(1), iftrue, iffalse));

			CompareNode(expectedast, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_IfStatement_NewScope_RedeclaredVariableThrows()
		{
			var tokens = new[] {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.ControlFlow, "if"),
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),
				new Token(TokenType.Grammar, "{"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, "="),
				new Token(TokenType.Immediate, "10"),
				new Token(TokenType.Grammar, "}"),
			};

			// Dont really need to compare but I had the code anyway
			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new VariableDeclarationNode("byte", "x"));
			var block = new BlockNode();
			block.AddChild(new VariableDeclarationNode("byte", "x"));
			block.AddChild(new VariableAssignmentNode("x", new ShortValueNode(10)));
			expectedast.AddChild(new IfNode(new ShortValueNode(1), block));

			CompareNode(expectedast, ast);
		}

		[TestMethod]
		public void CreateAST_MemoryAssignment_WithImmedate()
		{
			var tokens = Sharp_LR35902_Compiler.Lexer.GetTokenList("*31 = 10;");

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new MemoryAssignmentNode(31, new ShortValueNode(10)));
		}

		[TestMethod]
		public void CreateAST_MemoryAssignment_WithVariable()
		{
			var tokens = Sharp_LR35902_Compiler.Lexer.GetTokenList(new [] {
				"byte x = 10;",
				"*31 = x;"
			});

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(10)));
			expectedast.AddChild(new MemoryAssignmentNode(31, new VariableValueNode("x")));
		}

		[TestMethod]
		public void CreateAST_VariableAssignment_MemoryValue() {
			var tokens = Sharp_LR35902_Compiler.Lexer.GetTokenList("byte x = *100");

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedast.AddChild(new VariableAssignmentNode("x", new MemoryValueNode(100)));
		}

		[TestMethod]
		public void CreateExpression_SingleShort() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new ShortValueNode(1);

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Addition() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new AdditionNode(new ShortValueNode(1), new ShortValueNode(1));

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Addition_GetValue() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(2, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Addition_Multiple() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new AdditionNode(
				new AdditionNode(
					new ShortValueNode(1),
					new ShortValueNode(1)
				),
				new ShortValueNode(1)
			);

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new SubtractionNode(new ShortValueNode(1), new ShortValueNode(1));

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction_GetValue() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Subtraction_Multiple() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new SubtractionNode(
				new SubtractionNode(
					new ShortValueNode(1),
					new ShortValueNode(1)
				),
				new ShortValueNode(1)
			);

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction_With_Addition() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new SubtractionNode(
				new AdditionNode(
					new ShortValueNode(1),
					new ShortValueNode(1)
				),
				new ShortValueNode(1)
			);

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction_With_Addition_Value() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Parentheses() {
			var tokens = new[] {
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new AdditionNode(new ShortValueNode(1), new ShortValueNode(1));

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Parentheses_Order() {
			var tokens = new[] {
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),

				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),

				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")")
			};

			var expression = CreateExpression(tokens);

			var expected = new SubtractionNode(
				new AdditionNode(
					new ShortValueNode(1),
					new ShortValueNode(1)
				),
				new AdditionNode(
					new ShortValueNode(1),
					new ShortValueNode(1)
				)
			);

			CompareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_Parentheses_Order_Value() {
			var tokens = new[] {
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),

				new Token(TokenType.Operator, BuiltIn.Operators.Subtract),

				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_True_Value() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "true")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_False_Value() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "false")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Equals() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, "=="),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expected = new EqualsComparisonNode(
				new ShortValueNode(1),
				new ShortValueNode(1)
			);

			CompareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_Equals_Pass() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, "=="),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Equals_Fail() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, "=="),
				new Token(TokenType.Immediate, "2")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_LessThan() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, "<"),
				new Token(TokenType.Immediate, "2")
			};

			var expression = CreateExpression(tokens);

			var expected = new LessThanComparisonNode(
				new ShortValueNode(1),
				new ShortValueNode(2)
			);

			CompareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_LessThan_Pass() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, "<"),
				new Token(TokenType.Immediate, "2")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_LessThan_Fail() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "2"),
				new Token(TokenType.Comparison, "<"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_MoreThan() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "2"),
				new Token(TokenType.Comparison, ">"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expected = new MoreThanComparisonNode(
				new ShortValueNode(2),
				new ShortValueNode(1)
			);

			CompareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_MoreThan_Pass() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "2"),
				new Token(TokenType.Comparison, ">"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_MoreThan_Fail() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, ">"),
				new Token(TokenType.Immediate, "2")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Comparisons_Order() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, ">"),
				new Token(TokenType.Immediate, "2"),
				new Token(TokenType.Operator, BuiltIn.Operators.Add),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, "<"),
				new Token(TokenType.Immediate, "2")
			};

			var expression = CreateExpression(tokens);

			var expected = new AdditionNode(
				new MoreThanComparisonNode(
					new ShortValueNode(1),
					new ShortValueNode(2)
				),
				new LessThanComparisonNode(
					new ShortValueNode(1),
					new ShortValueNode(2)
				)
			);

			CompareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_Negate() {
			var tokens = new[] {
				new Token(TokenType.Operator, BuiltIn.Operators.Not),
				new Token(TokenType.Immediate, "0")
			};

			var expression = CreateExpression(tokens);

			var expected = new NegateNode(new ShortValueNode(0));

			CompareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_Negate_Value() {
			var tokens = new[] {
				new Token(TokenType.Operator, BuiltIn.Operators.Not),
				new Token(TokenType.Immediate, "0")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Negate_Variable()
		{
			var tokens = new[] {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "X"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, "X"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Operator, BuiltIn.Operators.Not),
				new Token(TokenType.Variable, "X")
			};

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new VariableDeclarationNode("byte", "X"));
			expectedast.AddChild(new VariableAssignmentNode("X", new ShortValueNode(1)));
			expectedast.AddChild(new VariableAssignmentNode("X", new NegateNode(new VariableValueNode("X"))));

			CompareNode(expectedast, ast);
		}

		[TestMethod]
		public void CreateExpression_And() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.And),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expected = new AndComparisonNode(
				new ShortValueNode(1),
				new ShortValueNode(1)
			);

			CompareNode(expected, expression);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateExpression_UnbalancedOperators() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				// No operator
				new Token(TokenType.Immediate, "1")
			};

			CreateExpression(tokens);
		}

		[TestMethod]
		public void CreateExpression_And_Value() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.And),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Or() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Or),
				new Token(TokenType.Immediate, "0")
			};

			var expression = CreateExpression(tokens);

			var expected = new OrComparisonNode(
				new ShortValueNode(1),
				new ShortValueNode(0)
			);

			CompareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_Or_Value() {
			var tokens = new[] {
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, BuiltIn.Operators.Or),
				new Token(TokenType.Immediate, "0")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}


		[TestMethod]
		public void CreateExpresssion_MultipleSubexpressions() {
			var tokens = new[] {
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Comparison, "=="),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "2"),
				new Token(TokenType.Comparison, ">"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")")
			};
			var expression = CreateExpression(tokens);

			Assert.AreEqual(4, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_MemoryValue() {
			var tokens = new[] {
				new Token(TokenType.Grammar, "*"), 
				new Token(TokenType.Immediate, "100"), 
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new MemoryValueNode(100);

			CompareNode(expectedexpression, expression);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateExpression_OnlyOperator() {
			var tokens = new[] {
				new Token(TokenType.Operator, "+")
			};

			CreateExpression(tokens);
		}

		[TestMethod]
		public void GetImmediateDataType_Smallest() {
			Assert.AreSame(BuiltIn.DataTypes.Byte, GetImmedateDataType(19));
			Assert.AreSame(BuiltIn.DataTypes.Int, GetImmedateDataType(256));
		}


		private static void CompareNode(Node expected, Node actual) {
			var actualchildren = actual.GetChildren().ToList();
			var expectedchilren = expected.GetChildren().ToList();

			if (actualchildren.Count != expectedchilren.Count)
				Assert.Fail("Nodes do not have the same number of child nodes.");

			for (var i = 0; i < expectedchilren.Count; i++)
				if (!actualchildren[i].Matches(expectedchilren[i]))
					Assert.Fail("A child is different than the expected.");

			for (var i = 0; i < actualchildren.Count; i++)
				CompareNode(expectedchilren[i], actualchildren[i]);
		}
	}
}
