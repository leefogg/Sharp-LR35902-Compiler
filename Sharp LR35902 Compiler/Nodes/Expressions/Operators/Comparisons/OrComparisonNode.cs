using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class OrComparisonNode : ComparisonNode
	{
		public OrComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public OrComparisonNode() { }

		protected override bool isTrue() => isTrue(Left.GetValue()) || isTrue(Right.GetValue());

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables)
		{
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode l && isTrue(l.GetValue()))
				return new ShortValueNode(booleanToShort(true));
			else if (right is ConstantNode r && isTrue(r.GetValue()))
				return new ShortValueNode(booleanToShort(true));

			return this;
		}
	}
}
