using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes.Assignment {
	public class MemoryAssignmentNode : AssignmentNode {
		public ushort Address { get; set; }

		public MemoryAssignmentNode(ushort address, ExpressionNode value) : base(value) {
			Address = address;
		}

		public override IEnumerable<string> GetUsedRegisterNames() => Value.GetUsedRegisterNames();

		public override bool Equals(object obj) {
			if (obj is MemoryAssignmentNode other)
				return Address == other.Address && other.Value.Equals(Value);

			return false;
		}

		public override Node[] GetChildren() => new Node[] { Value };
	}
}
