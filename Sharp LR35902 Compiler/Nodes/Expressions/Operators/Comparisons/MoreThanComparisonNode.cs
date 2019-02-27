namespace Sharp_LR35902_Compiler.Nodes
{
	public class MoreThanComparisonNode : ComparisonNode
	{
		public MoreThanComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public MoreThanComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() > Right.GetValue();
	}
}
