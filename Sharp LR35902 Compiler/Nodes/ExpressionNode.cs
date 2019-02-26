using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class ExpressionNode : Node
	{
		public abstract ushort GetValue();

		public override IEnumerable<string> GetUsedRegisterNames()
			=> NoRegisters;

		public static implicit operator ushort(ExpressionNode node)
			=> node.GetValue();
	}
}
