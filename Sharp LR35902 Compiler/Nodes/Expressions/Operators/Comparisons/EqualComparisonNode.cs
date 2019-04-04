using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class EqualsComparisonNode : ComparisonNode {
		public EqualsComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

		// Allow valueless construction
		public EqualsComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() == Right.GetValue();

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) {
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode && right is ConstantNode)
				return new ShortValueNode(BooleanToShort(IsTrue(left.GetValue()) == IsTrue(right.GetValue())));

			return new EqualsComparisonNode(left, right);
		}

		protected override string GetSymbol() => BuiltIn.Operators.Equal;
	}
}
