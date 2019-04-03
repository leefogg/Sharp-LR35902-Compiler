using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class AdditionNode : BinaryOperatorNode {
		public AdditionNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

		// Allow valueless construction
		public AdditionNode() { }

		public override ushort GetValue() => (ushort)(Left + Right);

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) {
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode && right is ConstantNode)
				return new ShortValueNode((ushort)(left.GetValue() + right.GetValue()));

			return new AdditionNode(left, right);
		}

		public override bool Matches(Node obj) {
			if (obj is AdditionNode other)
				return Left.Matches(other.Left) && Right.Matches(other.Right);

			return false;
		}
	}
}
