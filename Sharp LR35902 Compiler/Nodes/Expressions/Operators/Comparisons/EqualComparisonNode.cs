namespace Sharp_LR35902_Compiler.Nodes
{
	public class EqualsComparisonNode : ComparisonNode
	{
		public EqualsComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public EqualsComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() == Right.GetValue();
	}
}
