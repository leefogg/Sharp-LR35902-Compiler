namespace Sharp_LR35902_Compiler.Nodes
{
	public class OrComparisonOperatorNode : ComparisonNode
	{
		public OrComparisonOperatorNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public OrComparisonOperatorNode() { }

		protected override bool isTrue() => Left.GetValue() > 0 || Right.GetValue() > 0;
	}
}
