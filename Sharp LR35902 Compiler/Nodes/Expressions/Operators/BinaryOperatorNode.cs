using System.Collections.Generic;
using System.Linq;

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

		public override IEnumerable<string> GetReadVariables() => Left.GetReadVariables().Concat(Right.GetReadVariables());

		public override IEnumerable<Node> GetChildren() {
			yield return Left;
			yield return Right;
		}
	}
}
