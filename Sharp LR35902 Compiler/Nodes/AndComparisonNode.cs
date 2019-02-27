namespace Sharp_LR35902_Compiler.Nodes
{
	public class AndComparisonNode : ComparisonNode
	{
		public AndComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public AndComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() > 0 && Right.GetValue() > 0;
	}
}
