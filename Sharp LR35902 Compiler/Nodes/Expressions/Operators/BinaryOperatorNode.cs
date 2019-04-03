using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class BinaryOperatorNode : OperatorNode
	{
		public ExpressionNode Left { get; set; }
		public ExpressionNode Right { get; set; }

		protected BinaryOperatorNode(ExpressionNode left, ExpressionNode right)
		{
			Left = left;
			Right = right;
		}

		protected BinaryOperatorNode() { }

		public override IEnumerable<string> GetUsedRegisterNames()
		{
			foreach (var variablename in Left.GetUsedRegisterNames())
				yield return variablename;
			foreach (var variablename in Right.GetUsedRegisterNames())
				yield return variablename;
		}

		public override IEnumerable<Node> GetChildren() {
			yield return Left;
			yield return Right;
		}
	}
}
