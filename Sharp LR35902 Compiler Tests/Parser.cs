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
			var datatype = "byte";
			byte variablevalue = 42;
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ShortValueNode(variablevalue)));

			compareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_WithImmediate_CannotConvertToType()
		{
			var variablename = "x";
			var variablevalue = 500;
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_VariableExists()
		{
			var variablename = "x";
			var variablevalue = 42;
			var tokens = new List<Token>() {
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, variablevalue.ToString())
			};

			CreateAST(tokens);
		}

		[TestMethod]
		public void CreateAST_VariableAssignment_WithVariable()
		{
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, othervariablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Variable, othervariablename)
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableDeclarationNode(datatype, othervariablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new VariableValueNode(othervariablename)));
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_WithVariable_ValueDoesntExist()
		{
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Variable, "y")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_WithVariable_AssignedDoesntExist()
		{
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Variable, "y")
			};

			CreateAST(tokens);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_VariableAssignment_UnexpectedToken()
		{
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";"),
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

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));

			compareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareVariable_DataTypeDoesntExists()
		{
			var datatype = "blah";
			var variablename = "x";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Grammar, ";")
			};

			CreateAST(tokens);
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
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new ShortValueNode(variablevalue)));

			compareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_AdditionAssignment_WithVariable()
		{
			var tokens = new List<Token>
			{
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, "5"),

				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "+="),
				new Token(TokenType.Variable, "y")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedAST.AddChild(new VariableDeclarationNode("byte", "y"));
			expectedAST.AddChild(new VariableAssignmentNode("y", new ShortValueNode(5)));
			expectedAST.AddChild(new AdditionAssignmentNode("x", new VariableValueNode("y")));

			compareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_AdditionAssignment_WithImmediate()
		{
			var tokens = new List<Token>
			{
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
			expectedAST.AddChild(new AdditionAssignmentNode("x", new ShortValueNode(10)));

			compareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_SubtractionAssignment_WithVariable()
		{
			var tokens = new List<Token>
			{
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, "byte"),
				new Token(TokenType.Variable, "y"),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, "5"),

				new Token(TokenType.Variable, "x"),
				new Token(TokenType.Operator, "-="),
				new Token(TokenType.Variable, "y")
			};

			var ast = CreateAST(tokens);

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode("byte", "x"));
			expectedAST.AddChild(new VariableDeclarationNode("byte", "y"));
			expectedAST.AddChild(new VariableAssignmentNode("y", new ShortValueNode(5)));
			expectedAST.AddChild(new SubtractionAssignmentNode("x", new VariableValueNode("y")));

			compareNode(expectedAST, ast);
		}

		[TestMethod]
		public void CreateAST_SubtractionAssignment_WithImmediate()
		{
			var tokens = new List<Token>
			{
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
			expectedAST.AddChild(new SubtractionAssignmentNode("x", new ShortValueNode(10)));

			compareNode(expectedAST, ast);
		}


		[TestMethod]
		public void CreateAST_DeclareAndAssignVariable_WithVariable()
		{
			var datatype = "byte";
			var variablename = "x";
			var othervariablename = "y";
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, othervariablename),
				new Token(TokenType.Grammar, ";"),
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Variable, othervariablename)
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			var expectedAST = new ASTNode();
			expectedAST.AddChild(new VariableDeclarationNode(datatype, othervariablename));
			expectedAST.AddChild(new VariableDeclarationNode(datatype, variablename));
			expectedAST.AddChild(new VariableAssignmentNode(variablename, new VariableValueNode(othervariablename)));

			compareNode(expectedAST, ast);
		}

		[TestMethod]
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_WithVariable_DoesntExist()
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

			CreateAST(tokens);
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
		[ExpectedException(typeof(SyntaxException))]
		public void CreateAST_DeclareAndAssignVariable_CannotConvertImmediate()
		{
			var datatype = "byte";
			var variablename = "x";
			var variablevalue = 500; // Too large for a byte
			var tokens = new List<Token>() {
				new Token(TokenType.DataType, datatype),
				new Token(TokenType.Variable, variablename),
				new Token(TokenType.Operator, "="),
				new Token(TokenType.Immediate, variablevalue.ToString())
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

		[TestMethod]
		public void CreateAST_Label_CropsName()
		{
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "label:")
			};

			var ast = CreateAST(tokens);
			var children = ast.GetChildren();

			Assert.AreEqual("label", (children[0] as LabelNode).Name);
		}

		[TestMethod]
		public void CreateAST_Goto()
		{
			var labelname = "label";
			var tokens = new[] {
				new Token(TokenType.ControlFlow, "goto"),
				new Token(TokenType.Variable, labelname),
			};

			var ast = CreateAST(tokens);

			var expectedast = new ASTNode();
			expectedast.AddChild(new GotoNode(labelname));

			compareNode(expectedast, ast);
		}

		[TestMethod]
		public void CreateExpression_SingleShort()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new ShortValueNode(1);

			compareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Addition()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new AdditionNode(new ShortValueNode(1), new ShortValueNode(1));
			
			compareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Addition_GetValue()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(2, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Addition_Multiple()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
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

			compareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "-"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new SubtractionNode(new ShortValueNode(1), new ShortValueNode(1));

			compareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction_GetValue()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "-"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Subtraction_Multiple()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "-"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "-"),
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

			compareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction_With_Addition()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "-"),
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

			compareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Subtraction_With_Addition_Value()
		{
			var tokens = new[]
			{
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "-"),
				new Token(TokenType.Immediate, "1")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(1, expression.GetValue());
		}

		[TestMethod]
		public void CreateExpression_Parentheses()
		{
			var tokens = new[]
			{
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")")
			};

			var expression = CreateExpression(tokens);

			var expectedexpression = new AdditionNode(new ShortValueNode(1), new ShortValueNode(1));

			compareNode(expectedexpression, expression);
		}

		[TestMethod]
		public void CreateExpression_Parentheses_Order()
		{
			var tokens = new[]
			{
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),

				new Token(TokenType.Operator, "-"),

				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
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

			compareNode(expected, expression);
		}

		[TestMethod]
		public void CreateExpression_Parentheses_Order_Value()
		{
			var tokens = new[]
			{
				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")"),

				new Token(TokenType.Operator, "-"),

				new Token(TokenType.Grammar, "("),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Operator, "+"),
				new Token(TokenType.Immediate, "1"),
				new Token(TokenType.Grammar, ")")
			};

			var expression = CreateExpression(tokens);

			Assert.AreEqual(0, expression.GetValue());
		}

		[TestMethod]
		public void GetImmediateDataType_Smallest()
		{
			Assert.AreSame(BuiltIn.DataTypes.Byte, GetImmedateDataType(19));
			Assert.AreSame(BuiltIn.DataTypes.Int, GetImmedateDataType(256));
		}


		private void compareNode(Node expected, Node actual)
		{
			var actualchildren = actual.GetChildren();
			var expectedchilren = expected.GetChildren();

			if (actualchildren.Length != expectedchilren.Length)
				Assert.Fail();

			for (var i = 0; i < expectedchilren.Length; i++)
				if (!actualchildren[i].Equals(expectedchilren[i]))
					Assert.Fail();

			for (var i = 0; i < actualchildren.Length; i++)
				compareNode(expectedchilren[i], actualchildren[i]);
		}

	}
}
