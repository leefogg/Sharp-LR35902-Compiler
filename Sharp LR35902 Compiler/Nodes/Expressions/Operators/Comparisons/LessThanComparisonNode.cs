namespace Sharp_LR35902_Compiler.Nodes
{
	public class LessThanComparisonNode : ComparisonNode
	{
		public LessThanComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public LessThanComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() < Right.GetValue();
	}
}
