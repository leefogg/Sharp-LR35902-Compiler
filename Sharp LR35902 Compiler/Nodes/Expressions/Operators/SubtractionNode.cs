namespace Sharp_LR35902_Compiler.Nodes
{
	public class SubtractionNode : OperatorNode
	{
		public SubtractionNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public SubtractionNode() { }

		public override ushort GetValue()
			=> (ushort)(Left - Right);

		public override bool Equals(object obj)
		{
			if (obj is SubtractionNode other)
				return Left.Equals(other.Left) && Right.Equals(other.Right);

			return false;
		}
	}
}
