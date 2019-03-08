using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class AndComparisonNode : ComparisonNode
	{
		public AndComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public AndComparisonNode() { }

		protected override bool isTrue() => isTrue(Left.GetValue()) && isTrue(Right.GetValue());

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables)
		{
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode && right is ConstantNode)
				return new ShortValueNode(booleanToShort(isTrue(left.GetValue()) && isTrue(right.GetValue())));

			return new AndComparisonNode(left, right);
		}
	}
}
