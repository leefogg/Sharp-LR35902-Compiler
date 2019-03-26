using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class MoreThanComparisonNode : ComparisonNode
	{
		public MoreThanComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public MoreThanComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() > Right.GetValue();

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables)
		{
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode && right is ConstantNode)
				return new ShortValueNode(booleanToShort(left.GetValue() > right.GetValue()));

			return new MoreThanComparisonNode(left, right);
		}
	}
}
