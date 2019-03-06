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
			Left = Left.Optimize(knownvariables);
			Right = Right.Optimize(knownvariables);
			if (Left is ConstantNode && Right is ConstantNode)
				return new ShortValueNode(booleanToShort(Left.GetValue() > Right.GetValue()));

			return this;
		}
	}
}
