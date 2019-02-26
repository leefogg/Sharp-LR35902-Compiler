namespace Sharp_LR35902_Compiler.Nodes
{
	public class SubtractionNode : OperatorNode
	{
		public SubtractionNode(ExpressionNode left, ExpressionNode right)
		{
			Left = left;
			Right = right;
		}
		// Allow valueless construction
		public SubtractionNode()
		{
		}

		public override ushort GetValue()
			=> (ushort)(Left - Right);
	}
}
