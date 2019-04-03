using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class VariableAssignmentNode : AssignmentNode {
		public string VariableName { get; set; }
		

		public VariableAssignmentNode(string variablename, ExpressionNode value) : base(value) {
			VariableName = variablename;
		}

		public override IEnumerable<string> GetUsedRegisterNames() {
			yield return VariableName;

			foreach (var usedvariable in Value.GetUsedRegisterNames())
				yield return usedvariable;
		}

		public override bool Matches(Node obj) {
			if (obj is VariableAssignmentNode other)
				return other.VariableName == VariableName && other.Value.Matches(Value);

			return false;
		}

		public override IEnumerable<Node> GetChildren() { yield return Value; }
	}
}
