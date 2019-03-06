using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class AdditionNode : OperatorNode
	{
		public AdditionNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
		// Allow valueless construction
		public AdditionNode() { }

		public override ushort GetValue()
			=> (ushort)(Left + Right);

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables)
		{
			Left = Left.Optimize(knownvariables);
			Right = Right.Optimize(knownvariables);
			if (Left is ConstantNode && Right is ConstantNode)
				return new ShortValueNode((ushort)(Left.GetValue() + Right.GetValue()));

			return this;
		}

		public override bool Equals(object obj)
		{
			if (obj is AdditionNode other)
				return Left.Equals(other.Left) && Right.Equals(other.Right);

			return false;
		}
	}
}
