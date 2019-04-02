using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class AssignmentNode : Node {
		public ExpressionNode Value { get; set; }

		public AssignmentNode(ExpressionNode value) { Value = value; }
	}
}
