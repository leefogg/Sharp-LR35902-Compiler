using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public abstract class ComparisonNode : Node
	{
		public ValueNode Left { get; }
		public ValueNode Right { get; }

		public ComparisonOperator @Operator { get; }

		public ComparisonNode(ValueNode left, ComparisonOperator op, ValueNode right)
		{
			Left = left;
			Right = right;
			Operator = op;
		}
    }
}
