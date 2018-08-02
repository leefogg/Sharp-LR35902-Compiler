using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class IfNode : Node
    {
		public ComparisonNode Condition { get; }

		public IfNode(ComparisonNode condition)
		{

		}
    }
}
