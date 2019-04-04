using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler.Nodes;
using static Sharp_LR35902_Compiler.Optimizer;

namespace Sharp_LR35902_Compiler_Tests {
	[TestClass]
	public class Optimizer {
		[TestMethod]
		public void RemoveUnusedVariables_DeclaredButNotUsed_Removes() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));

			RemoveUnusedVariables(ast);

			Assert.AreEqual(0, ast.GetChildren().Count());
		}

		[TestMethod]
		public void RemoveUnusedVariables_WriteButNoRead_Removes() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));

			RemoveUnusedVariables(ast);

			Assert.AreEqual(0, ast.GetChildren().Count());
		}

		[TestMethod]
		public void RemoveUnusedVariables_ReadOnce_Remains() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			ast.AddChild(new VariableAssignmentNode("b", new VariableValueNode("a")));

			RemoveUnusedVariables(ast);

			Assert.AreEqual(2, ast.GetChildren().Count());
		}

		[TestMethod]
		public void RemoveUnusedVariables_ReducesChain() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new VariableDeclarationNode("byte", "b"));
			ast.AddChild(new VariableAssignmentNode("b", new VariableValueNode("a")));
			ast.AddChild(new VariableDeclarationNode("byte", "c"));
			ast.AddChild(new VariableAssignmentNode("c", new VariableValueNode("b")));

			var iterations = 0;
			while (RemoveUnusedVariables(ast))
				iterations++;

			Assert.AreEqual(3, iterations);
			Assert.AreEqual(0, ast.GetChildren().Count());
		}

		[TestMethod]
		public void PropagateConstants_CopiesOver() {
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
		public void PropagateConstants_ResolvesExpressions() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			var expression = new VariableAssignmentNode("a", new AdditionNode(new ShortValueNode(1), new ShortValueNode(5)));
			ast.AddChild(expression);

			var changed = PropagateConstants(ast);

			Assert.IsTrue(changed);
			Assert.IsInstanceOfType(expression.Value, typeof(ConstantNode));
		}

		[TestMethod]
		public void PropagateConstants_ResolvesExpressions_WithVariable() {
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
		public void PropagateConstants_DetectsSubNodeChanges() {
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
		public void PropagateConstants_UsesLatestValue() {
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
		public void PropagateConstants_Chains() {
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
		public void PropagateConstants_RemovesIncrement() {
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
			Assert.AreEqual(4, ast.GetChildren().Count());
		}

		[TestMethod]
		public void PropagateConstants_RemovesIncrement_Multiple() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new IncrementNode("a"));
			ast.AddChild(new IncrementNode("a"));

			PropagateConstants(ast);

			Assert.AreEqual(2, ast.GetChildren().Count());
		}

		[TestMethod]
		public void PropagateConstants_RemovesDecrement_Multiple() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "a"));
			ast.AddChild(new VariableAssignmentNode("a", new ShortValueNode(5)));
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new DecrementNode("a"));
			ast.AddChild(new DecrementNode("a"));

			PropagateConstants(ast);

			Assert.AreEqual(2, ast.GetChildren().Count());
		}

		[TestMethod]
		public void PropagateConstants_Increment_BeforeAndAfter() {
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
		public void PropagateConstants_RemovesDecrement() {
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
			Assert.AreEqual(4, ast.GetChildren().Count());
		}

		[TestMethod]
		public void PropagateConstants_Decrement_BeforeAndAfter() {
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

		[TestMethod]
		public void PropagteConstants_If_ExtractsTrueBody()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(1)));
			var block = new BlockNode();
			block.AddChild(new VariableDeclarationNode("byte", "a"));
			block.AddChild(new VariableAssignmentNode("a", new ShortValueNode(1)));
			ast.AddChild(new IfNode(new VariableValueNode("x"), block));

			var changesmade = PropagateConstants(ast);

			Assert.IsTrue(changesmade);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(4, children.Count);
			Assert.IsInstanceOfType(children[2], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[3], typeof(VariableAssignmentNode));
		}

		[TestMethod]
		public void PropagteConstants_If_ExtractsFalseBody()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			var iftrue = new BlockNode();
			iftrue.AddChild(new VariableAssignmentNode("x", new ShortValueNode(1)));
			var iffalse = new BlockNode();
			iffalse.AddChild(new AdditionAssignmentNode("x", new ShortValueNode(2)));
			ast.AddChild(new IfNode(new VariableValueNode("x"), iftrue, iffalse));

			var changesmade = PropagateConstants(ast);

			Assert.IsTrue(changesmade);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(3, children.Count);
			Assert.IsInstanceOfType(children[2], typeof(AdditionAssignmentNode));
		}

		[TestMethod]
		public void PropagteConstants_If_ExtractsBody_Recursive()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(1)));
			var subblock = new BlockNode();
			subblock.AddChild(new VariableDeclarationNode("byte", "a"));
			subblock.AddChild(new VariableAssignmentNode("a", new ShortValueNode(1)));
			var block = new BlockNode();
			block.AddChild(new IfNode(new VariableValueNode("x"), subblock));
			ast.AddChild(new IfNode(new VariableValueNode("x"), block));

			var changesmade = PropagateConstants(ast);

			Assert.IsTrue(changesmade);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(4, children.Count);
			Assert.IsInstanceOfType(children[2], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[3], typeof(VariableAssignmentNode));
		}

		[TestMethod]
		public void Transform_AdditionAssignmentToExpression() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new AdditionAssignmentNode("x", new ShortValueNode(5)));

			var changed = TransformAdditionAssignmentToExpression(ast);

			Assert.IsTrue(changed);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(2, children.Count);
			Assert.IsInstanceOfType(children[1], typeof(VariableAssignmentNode));
			var assignment = (VariableAssignmentNode)children[1];
			Assert.IsInstanceOfType(assignment.Value, typeof(AdditionNode));
			Assert.IsInstanceOfType(((AdditionNode)assignment.Value).Left, typeof(VariableValueNode));
			Assert.IsInstanceOfType(((AdditionNode)assignment.Value).Right, typeof(ShortValueNode));
		}

		[TestMethod]
		public void Transform_SubtractionAssignmentToExpression() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new SubtractionAssignmentNode("x", new ShortValueNode(5)));

			var changed = TransformSubtractionAssignmentToExpression(ast);

			var children = ast.GetChildren().ToList();
			Assert.IsTrue(changed);
			Assert.AreEqual(2, children.Count);
			Assert.IsInstanceOfType(children[1], typeof(VariableAssignmentNode));
			var assignment = (VariableAssignmentNode)children[1];
			Assert.IsInstanceOfType(assignment.Value, typeof(SubtractionNode));
			Assert.IsInstanceOfType(((SubtractionNode)assignment.Value).Left, typeof(VariableValueNode));
			Assert.IsInstanceOfType(((SubtractionNode)assignment.Value).Right, typeof(ShortValueNode));
		}

		[TestMethod]
		public void FlattenExpression_Assignment_Constant_NotAffected() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new SubtractionAssignmentNode("x", new ShortValueNode(5)));

			var changes = FlattenExpression(ast, 1);

			Assert.IsTrue(changes == 0);
		}

		[TestMethod]
		public void FlattenExpression_Assignment_FlatExpression_NotAffected() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new SubtractionAssignmentNode("x", new AdditionNode(new ShortValueNode(5), new ShortValueNode(1))));

			var changes = FlattenExpression(ast, 1);

			Assert.IsTrue(changes == 0);
		}

		[TestMethod]
		public void FlattenExpression_Assignment_SubexpressionExtracted() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new AdditionNode(
				new AdditionNode(
					new ShortValueNode(5),
					new ShortValueNode(5)
				),
				new ShortValueNode(1)
			)));

			var changes = FlattenExpression(ast, 1);

			Assert.IsTrue(changes > 0);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(4, children.Count);
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[1], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[2], typeof(VariableAssignmentNode));
			Assert.IsInstanceOfType(children[3], typeof(VariableAssignmentNode));
		}

		[TestMethod]
		public void FlattenExpression_Assignment_SideSubexpressionExtracted() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new SubtractionNode(
				new AdditionNode(
					new ShortValueNode(1),
					new ShortValueNode(2)
				),
				new AdditionNode(
					new ShortValueNode(3),
					new ShortValueNode(4)
				)
			)));

			var changes = FlattenExpression(ast, 1);

			Assert.IsTrue(changes > 0);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(6, children.Count);
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[1], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[2], typeof(VariableAssignmentNode));
			Assert.IsInstanceOfType(children[3], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[4], typeof(VariableAssignmentNode));
			Assert.IsInstanceOfType(children[5], typeof(VariableAssignmentNode));
		}

		[TestMethod]
		public void FlattenExpressions_Assignment_FlattensAll() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new SubtractionNode(
				new AdditionNode(
					new ShortValueNode(1),
					new ShortValueNode(2)
				),
				new ShortValueNode(3)
			)));
			ast.AddChild(new VariableDeclarationNode("byte", "y"));
			ast.AddChild(new VariableAssignmentNode("y", new SubtractionNode(
				new AdditionNode(
					new ShortValueNode(1),
					new ShortValueNode(2)
				),
				new ShortValueNode(3)
			)));

			var changed = FlattenExpressions(ast);

			Assert.IsTrue(changed);
		}

		[TestMethod]
		public void FlattenExpresison_Assignment_If_ConditionExtracts() {
			var ast = new ASTNode();
			var ifnode = new IfNode(
				new AdditionNode(
					new ShortValueNode(0),
					new ShortValueNode(1)
				), new BlockNode()
			);
			ast.AddChild(ifnode);

			var changes = FlattenExpression(ast, 0);

			Assert.IsTrue(changes > 0);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(3, children.Count);
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(ifnode.Condition, typeof(VariableValueNode));
		}

		[TestMethod]
		public void FlattenExpression_Assignment_MemoryValue_Left()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			var additionnode = new AdditionNode(new MemoryValueNode(100), new ShortValueNode(10));
			var assignment = new VariableAssignmentNode("x", additionnode);
			ast.AddChild(assignment);

			var changes = FlattenExpression(ast, 1);

			Assert.IsTrue(changes > 0);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(4, children.Count);
			Assert.IsInstanceOfType(additionnode.Left, typeof(VariableValueNode));
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[1], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[2], typeof(VariableAssignmentNode));
			Assert.IsInstanceOfType(children[3], typeof(VariableAssignmentNode));
			Assert.AreSame(children[3], assignment);
		}

		[TestMethod]
		public void FlattenExpression_Assignment_MemoryValue_Right()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			var additionnode = new AdditionNode(new ShortValueNode(10), new MemoryValueNode(100));
			var assignment = new VariableAssignmentNode("x", additionnode);
			ast.AddChild(assignment);

			var changes = FlattenExpression(ast, 1);

			Assert.IsTrue(changes > 0);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(4, children.Count);
			Assert.IsInstanceOfType(additionnode.Right, typeof(VariableValueNode));
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[1], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[2], typeof(VariableAssignmentNode));
			Assert.IsInstanceOfType(children[3], typeof(VariableAssignmentNode));
			Assert.AreSame(children[3], assignment);
		}

		[TestMethod]
		public void FlattenExpression_Assignment_MemoryValue_Both()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			var additionnode = new AdditionNode(new MemoryValueNode(100), new MemoryValueNode(100));
			var assignment = new VariableAssignmentNode("x", additionnode);
			ast.AddChild(assignment);

			var changes = FlattenExpression(ast, 1);

			Assert.IsTrue(changes > 0);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(6, children.Count);
			Assert.IsInstanceOfType(additionnode.Left, typeof(VariableValueNode));
			Assert.IsInstanceOfType(additionnode.Right, typeof(VariableValueNode));
			Assert.IsInstanceOfType(children[0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[1], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[2], typeof(VariableAssignmentNode));
			Assert.IsInstanceOfType(children[3], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(children[4], typeof(VariableAssignmentNode));
			Assert.IsInstanceOfType(children[5], typeof(VariableAssignmentNode));
			Assert.AreSame(children[5], assignment);
			Assert.AreNotEqual(((VariableAssignmentNode)children[2]).VariableName, ((VariableAssignmentNode)children[4]).VariableName);
		}

		[TestMethod]
		public void TransformToIncDec_Increment_Left_Optimizes() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			ast.AddChild(new VariableAssignmentNode("x", new AdditionNode(new ShortValueNode(1), new VariableValueNode("x"))));

			var changesmade = TransformToIncDec(ast);

			Assert.IsTrue(changesmade);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(3, children.Count);
			Assert.IsInstanceOfType(children[2], typeof(IncrementNode));
		}

		[TestMethod]
		public void TransformToIncDec_Increment_Right_Optimizes()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			ast.AddChild(new VariableAssignmentNode("x", new AdditionNode(new VariableValueNode("x"), new ShortValueNode(1))));

			var changesmade = TransformToIncDec(ast);

			Assert.IsTrue(changesmade);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(3, children.Count);
			Assert.IsInstanceOfType(children[2], typeof(IncrementNode));
		}

		[TestMethod]
		public void TransformToIncDec_Subtract_Left_Optimizes()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			ast.AddChild(new VariableAssignmentNode("x", new SubtractionNode(new ShortValueNode(1), new VariableValueNode("x"))));

			var changesmade = TransformToIncDec(ast);

			Assert.IsTrue(changesmade);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(3, children.Count);
			Assert.IsInstanceOfType(children[2], typeof(DecrementNode));
		}

		[TestMethod]
		public void TransformToIncDec_Subtract_Right_Optimizes()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(0)));
			ast.AddChild(new VariableAssignmentNode("x", new SubtractionNode(new VariableValueNode("x"), new ShortValueNode(1))));

			var changesmade = TransformToIncDec(ast);

			Assert.IsTrue(changesmade);
			var children = ast.GetChildren().ToList();
			Assert.AreEqual(3, children.Count);
			Assert.IsInstanceOfType(children[2], typeof(DecrementNode));
		}


		[TestMethod]
		public void CreateBasicBlocks_OneBlock() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableDeclarationNode("byte", "y"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5)));

			var blocks = CreateBasicBlocks(ast).ToArray();

			Assert.AreEqual(1, blocks.Length);
			Assert.AreEqual(3, blocks[0].Count);
		}

		[TestMethod]
		public void CreateBasicBlocks_LabelSeperates() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5)));
			ast.AddChild(new LabelNode("label1"));
			ast.AddChild(new VariableDeclarationNode("byte", "y"));

			var blocks = CreateBasicBlocks(ast).ToArray();

			Assert.AreEqual(2, blocks.Length);
			Assert.AreEqual(2, blocks[0].Count);
			Assert.AreEqual(2, blocks[1].Count);
			Assert.IsInstanceOfType(blocks[0][0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(blocks[1][0], typeof(LabelNode));
		}

		[TestMethod]
		public void CreateBasicBlocks_JumpSeperates() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5)));
			ast.AddChild(new GotoNode("label1"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(10)));

			var blocks = CreateBasicBlocks(ast).ToArray();

			Assert.AreEqual(2, blocks.Length);
			Assert.AreEqual(3, blocks[0].Count);
			Assert.AreEqual(1, blocks[1].Count);
			Assert.IsInstanceOfType(blocks[0][0], typeof(VariableDeclarationNode));
			Assert.IsInstanceOfType(blocks[1][0], typeof(VariableAssignmentNode));
		}

		[TestMethod]
		public void CreateBasicBlocks_NoEmpty() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5)));
			ast.AddChild(new GotoNode("label1"));
			ast.AddChild(new LabelNode("label1"));
			ast.AddChild(new VariableDeclarationNode("byte", "y"));

			var blocks = CreateBasicBlocks(ast).ToArray();

			Assert.AreEqual(2, blocks.Length);
		}
	}
}
