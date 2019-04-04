using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler.Nodes {
	public class VariableAssignmentNode : AssignmentNode {
		public string VariableName { get; set; }
		
		public VariableAssignmentNode(string variablename, ExpressionNode value) : base(value) {
			VariableName = variablename;
		}

		public override IEnumerable<string> GetWrittenVaraibles() { yield return VariableName; }
		public override IEnumerable<string> GetReadVariables() => Value.GetReadVariables();

		public override bool Matches(Node obj) {
			if (obj is VariableAssignmentNode other)
				return other.VariableName == VariableName && other.Value.Matches(Value);

			return false;
		}

		public override IEnumerable<Node> GetChildren() { yield return Value; }

		public override string ToString() => VariableName + ' ' + BuiltIn.Operators.Equal + ' ' + Value.ToString();
	}
}
