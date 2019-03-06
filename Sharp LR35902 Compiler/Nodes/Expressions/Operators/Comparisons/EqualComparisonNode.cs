using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class EqualsComparisonNode : ComparisonNode
	{
		public EqualsComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public EqualsComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() == Right.GetValue();

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables)
		{
			Left = Left.Optimize(knownvariables);
			Right = Right.Optimize(knownvariables);
			if (Left is ConstantNode && Right is ConstantNode)
				return new ShortValueNode(booleanToShort(isTrue(Left.GetValue()) == isTrue(Right.GetValue())));

			return this;
		}
	}
}
