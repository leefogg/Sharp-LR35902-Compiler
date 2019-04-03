using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class ExpressionNode : Node {
		public abstract ushort GetValue();
		public abstract ExpressionNode Optimize(IDictionary<string, ushort> knownvariables);

		public override IEnumerable<string> GetWrittenVaraibles() => NoVariables;

		public static implicit operator ushort(ExpressionNode node) => node.GetValue();

		public static bool IsTrue(ushort value) => value > 0;

		public static ushort BooleanToShort(bool value) => value ? (ushort)1 : (ushort)0;
	}
}
