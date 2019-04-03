using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class IncrementNode : Node {
		public string VariableName { get; }

		public IncrementNode(string variablename) { VariableName = variablename; }

		public override bool Matches(Node obj) {
			if (obj is IncrementNode other)
				return other.VariableName == VariableName;

			return false;
		}

		public override IEnumerable<string> GetWrittenVaraibles() { yield return VariableName; }
		public override IEnumerable<string> GetReadVariables() { yield return VariableName; }

		public override IEnumerable<Node> GetChildren() => NoChildren;
	}
}
