using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using static Sharp_LR35902_Compiler.Optimizer;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Optimizer
	{
		[TestMethod]
		public void RemoveUnusedVariables_DeclaredButNotUsed_Removes()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));

			RemoveUnusedVariables(ast);

			Assert.AreEqual(0, ast.GetChildren().Length);
		}

		[TestMethod]
		public void RemoveUnusedVariables_WriteButNoRead_Removes()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));

			RemoveUnusedVariables(ast);

			Assert.AreEqual(0, ast.GetChildren().Length);
		}

		[TestMethod]
		public void RemoveUnusedVariables_ReadOnce_Remains()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			ast.AddChild(new VariableAssignmentNode("b", new VariableValueNode("a")));

			RemoveUnusedVariables(ast);

			Assert.AreEqual(2, ast.GetChildren().Length);
		}

		[TestMethod]
		public void RemoveUnusedVariables_ReducesChain()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			ast.AddChild(new VariableAssignmentNode("b", new VariableValueNode("a")));
			ast.AddChild(new VariableDeclarationNode("byte", "c"));
			ast.AddChild(new VariableAssignmentNode("c", new VariableValueNode("b")));

			var iterations = 0;
			while(RemoveUnusedVariables(ast))
				iterations++;

			Assert.AreEqual(3, iterations);
			Assert.AreEqual(0, ast.GetChildren().Length);
		}

		[TestMethod]
		public void PropagateConstants_CopiesOver()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			var assignment = new VariableAssignmentNode("b", new VariableValueNode("a"));
			ast.AddChild(assignment);

			PropagateConstants(ast);

			Assert.IsInstanceOfType(assignment.Value, typeof(ShortValueNode));
			Assert.AreEqual(5, ((ShortValueNode)assignment.Value).GetValue());
		}

		[TestMethod]
		public void PropagateConstants_ResolvesExpressions()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			var expression = new VariableAssignmentNode("a", new AdditionNode(new ShortValueNode(1), new ShortValueNode(5)));
			ast.AddChild(expression);

			var changed = PropagateConstants(ast);

			Assert.IsTrue(changed);
			Assert.IsInstanceOfType(expression.Value, typeof(ConstantNode));
		}

		[TestMethod]
		public void PropagateConstants_ResolvesExpressions_WithVariable()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			var expression = new VariableAssignmentNode("a", new AdditionNode(new ShortValueNode(1), new VariableValueNode("x")));
			ast.AddChild(expression);

			var changed = PropagateConstants(ast);

			Assert.IsTrue(changed);
		}

		[TestMethod]
		public void PropagateConstants_DetectsSubNodeChanges()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			var value = new AdditionNode(new VariableValueNode("y"), new VariableValueNode("x"));
			ast.AddChild(new VariableAssignmentNode("a", value));

			var changed = PropagateConstants(ast);

			Assert.IsTrue(changed);
		}

		[TestMethod]
		public void PropagateConstants_UsesLatestValue()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(10)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			var assignment = new VariableAssignmentNode("b", new VariableValueNode("a"));
			ast.AddChild(assignment);

			PropagateConstants(ast);

			Assert.IsInstanceOfType(assignment.Value, typeof(ShortValueNode));
			Assert.AreEqual(10, ((ShortValueNode)assignment.Value).GetValue());
		}

		[TestMethod]
		public void PropagateConstants_Chains()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			ast.AddChild(new VariableAssignmentNode("b", new VariableValueNode("a")));
			ast.AddChild(new VariableDeclarationNode("byte", "c"));
			ast.AddChild(new VariableAssignmentNode("c", new VariableValueNode("b")));
			ast.AddChild(new VariableDeclarationNode("byte", "c"));
			var assignment = new VariableAssignmentNode("c", new VariableValueNode("b"));
			ast.AddChild(assignment);

			var iterations = 0;
			while (PropagateConstants(ast))
				iterations++;

			Assert.AreEqual(1, iterations);
			Assert.IsInstanceOfType(assignment.Value, typeof(ShortValueNode));
			Assert.AreEqual(5, ((ShortValueNode)assignment.Value).GetValue());
		}

		[TestMethod]
		public void PropagateConstants_RemovesIncrement()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			var assignment = new VariableAssignmentNode("b", new VariableValueNode("a"));
			ast.AddChild(assignment);

			PropagateConstants(ast);

			Assert.IsInstanceOfType(assignment.Value, typeof(ShortValueNode));
			Assert.AreEqual(6, ((ShortValueNode)assignment.Value).GetValue());
			Assert.AreEqual(4, ast.GetChildren().Length);
		}

		[TestMethod]
		public void PropagateConstants_RemovesIncrement_Multiple()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new IncrementNode("a"));

			PropagateConstants(ast);

			Assert.AreEqual(2, ast.GetChildren().Length);
		}

		[TestMethod]
		public void PropagateConstants_RemovesDecrement_Multiple()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new DecrementNode("a"));

			PropagateConstants(ast);

			Assert.AreEqual(2, ast.GetChildren().Length);
		}

		[TestMethod]
		public void PropagateConstants_Increment_BeforeAndAfter()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			var before = new VariableAssignmentNode("b", new VariableValueNode("a"));
			ast.AddChild(before);
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new VariableDeclarationNode("byte", "c"));
			var after = new VariableAssignmentNode("c", new VariableValueNode("a"));
			ast.AddChild(after);

			PropagateConstants(ast);

			Assert.IsInstanceOfType(before.Value, typeof(ShortValueNode));
			Assert.AreEqual(5, ((ShortValueNode)before.Value).GetValue());
			Assert.IsInstanceOfType(after.Value, typeof(ShortValueNode));
			Assert.AreEqual(6, ((ShortValueNode)after.Value).GetValue());
		}

		[TestMethod]
		public void PropagateConstants_RemovesDecrement()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			var assignment = new VariableAssignmentNode("b", new VariableValueNode("a"));
			ast.AddChild(assignment);

			PropagateConstants(ast);

			Assert.IsInstanceOfType(assignment.Value, typeof(ShortValueNode));
			Assert.AreEqual(4, ((ShortValueNode)assignment.Value).GetValue());
			Assert.AreEqual(4, ast.GetChildren().Length);
		}

		[TestMethod]
		public void PropagateConstants_Decrement_BeforeAndAfter()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			var before = new VariableAssignmentNode("b", new VariableValueNode("a"));
			ast.AddChild(before);
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new VariableDeclarationNode("byte", "c"));
			var after = new VariableAssignmentNode("c", new VariableValueNode("a"));
			ast.AddChild(after);

			PropagateConstants(ast);

			Assert.IsInstanceOfType(before.Value, typeof(ShortValueNode));
			Assert.AreEqual(5, ((ShortValueNode)before.Value).GetValue());
			Assert.IsInstanceOfType(after.Value, typeof(ShortValueNode));
			Assert.AreEqual(4, ((ShortValueNode)after.Value).GetValue());
		}
	}
}
