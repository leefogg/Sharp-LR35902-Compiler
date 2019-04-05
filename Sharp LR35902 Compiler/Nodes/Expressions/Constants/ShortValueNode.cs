using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class ShortValueNode : ConstantNode {
		public readonly ushort Value;

		public ShortValueNode(ushort value) { Value = value; }

		public override bool Matches(Node obj) {
			if (obj is ShortValueNode other)
				return other.Value == Value;

			return false;
		}

		public override ushort GetValue() => Value;

		public override string ToString() => Value.ToString();
	}
}
