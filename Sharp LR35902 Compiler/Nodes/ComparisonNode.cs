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

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            foreach (var variablename in Left.GetUsedRegisterNames())
                yield return variablename;
            foreach (var variablename in Right.GetUsedRegisterNames())
                yield return variablename;
        }

		public override bool Equals(object obj)
		{
			if (obj is ComparisonNode other)
				return other.Left.Equals(Left) && other.Right.Equals(Right);

			return false;
		}
	}
}
