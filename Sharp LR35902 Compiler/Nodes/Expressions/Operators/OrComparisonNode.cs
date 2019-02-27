namespace Sharp_LR35902_Compiler.Nodes
{
	public class OrComparisonNode : ComparisonNode
	{
		public OrComparisonNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public OrComparisonNode() { }

		protected override bool isTrue() => Left.GetValue() > 0 || Right.GetValue() > 0;
	}
}
