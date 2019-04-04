using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class OrComparisonNode : ComparisonNode {
		public OrComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

		// Allow valueless construction
		public OrComparisonNode() { }

		protected override bool isTrue() => IsTrue(Left.GetValue()) || IsTrue(Right.GetValue());

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) {
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode l && IsTrue(l.GetValue()))
				return new ShortValueNode(BooleanToShort(true));
			if (right is ConstantNode r && IsTrue(r.GetValue()))
				return new ShortValueNode(BooleanToShort(true));

			return this;
		}

		protected override string GetSymbol() => BuiltIn.Operators.Or;
	}
}
