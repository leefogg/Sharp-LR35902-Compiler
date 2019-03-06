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
			Left = Left.Optimize(knownvariables);
			Right = Right.Optimize(knownvariables);
			if (Left is ConstantNode && Right is ConstantNode)
				return new ShortValueNode(booleanToShort(isTrue(Left.GetValue()) && isTrue(Right.GetValue())));

			return this;
		}
	}
}
