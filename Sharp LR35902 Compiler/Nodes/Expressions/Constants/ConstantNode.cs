using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class ConstantNode : ValueNode
	{
		// Optimizations stop here. Cant get simpler than a constant
		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) => this;
	}
}
