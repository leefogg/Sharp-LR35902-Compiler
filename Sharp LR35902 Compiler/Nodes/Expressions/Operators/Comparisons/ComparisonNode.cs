﻿namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class ComparisonNode : OperatorNode {
		public ComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

		// Allow valueless construction
		public ComparisonNode() { }

		public override ushort GetValue() => booleanToShort(isTrue());

		protected abstract bool isTrue();

		public override bool Equals(object obj) {
			if (obj is ComparisonNode other)
				return Left.Equals(other.Left) && Right.Equals(other.Right);

			return false;
		}
	}
}
