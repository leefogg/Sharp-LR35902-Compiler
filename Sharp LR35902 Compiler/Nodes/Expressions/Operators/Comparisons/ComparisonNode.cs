namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class ComparisonNode : BinaryOperatorNode {
		protected ComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

		// Allow valueless construction
		protected ComparisonNode() { }

		public override ushort GetValue() => BooleanToShort(isTrue());

		protected abstract bool isTrue();

		public override bool Matches(Node obj) {
			if (obj is ComparisonNode other)
				return Left.Matches(other.Left) && Right.Matches(other.Right);

			return false;
		}
	}
}
