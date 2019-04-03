using System;
using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class MemoryValueNode : ValueNode {
		public ushort Address { get; set; }

		public MemoryValueNode(ushort address) { Address = address; }

		public override ushort GetValue() { throw new NotSupportedException("Function only available as a run-time feature."); }
		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) => this;

		public override bool Matches(Node obj) {
			if (obj is MemoryValueNode other)
				return Address == other.Address;

			return false;
		}
	}
}
