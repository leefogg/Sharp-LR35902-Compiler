using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class ExpressionNode : Node
	{
		public abstract ushort GetValue();
		public abstract ExpressionNode Optimize(IDictionary<string, ushort> knownvariables);

		public override IEnumerable<string> GetUsedRegisterNames()
			=> NoRegisters;

		public static implicit operator ushort(ExpressionNode node)
			=> node.GetValue();

		protected static bool isTrue(ushort value) => value > 0;
		protected static ushort booleanToShort(bool value) => value ? (ushort)1 : (ushort)0;
	}
}
