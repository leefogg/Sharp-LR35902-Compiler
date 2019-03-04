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
	}
}
