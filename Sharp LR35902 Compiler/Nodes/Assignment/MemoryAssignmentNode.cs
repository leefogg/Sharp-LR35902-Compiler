using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes.Assignment {
	public class MemoryAssignmentNode : AssignmentNode {
		public ushort Address { get; set; }

		public MemoryAssignmentNode(ushort address, ExpressionNode value) : base(value) {
			Address = address;
		}

		public override IEnumerable<string> GetWrittenVaraibles() => NoVariables;
		public override IEnumerable<string> GetReadVariables() => Value.GetReadVariables();

		public override bool Matches(Node obj) {
			if (obj is MemoryAssignmentNode other)
				return Address == other.Address && other.Value.Matches(Value);

			return false;
		}

		public override IEnumerable<Node> GetChildren() { yield return Value; }
	}
}
