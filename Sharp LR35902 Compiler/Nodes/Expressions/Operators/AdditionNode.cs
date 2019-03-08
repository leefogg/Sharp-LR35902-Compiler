﻿using System.Collections.Generic;

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
			var left = Left.Optimize(knownvariables);
			var right = Right.Optimize(knownvariables);
			if (left is ConstantNode && right is ConstantNode)
				return new ShortValueNode((ushort)(left.GetValue() + right.GetValue()));

			return new AdditionNode(left, right);
		}

		public override bool Equals(object obj)
		{
			if (obj is AdditionNode other)
				return Left.Equals(other.Left) && Right.Equals(other.Right);

			return false;
		}
	}
}
