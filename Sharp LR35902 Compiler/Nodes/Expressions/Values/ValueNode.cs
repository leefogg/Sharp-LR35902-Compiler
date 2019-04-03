using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class ValueNode : ExpressionNode {
		public override IEnumerable<Node> GetChildren() => NoChildren;
	}
}
