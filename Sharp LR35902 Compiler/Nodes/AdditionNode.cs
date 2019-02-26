namespace Sharp_LR35902_Compiler.Nodes
{
	public class AdditionNode : OperatorNode
	{
		public AdditionNode(ExpressionNode left, ExpressionNode right)
		{
			Left = left;
			Right = right;
		}
		// Allow valueless construction
		public AdditionNode()
		{
		}

		public override ushort GetValue()
			=> (ushort)(Left + Right);

		public override bool Equals(object obj)
		{
			if (obj is AdditionNode other)
				return Left.Equals(other.Left) && Right.Equals(other.Right);

			return false;
		}
	}
}
