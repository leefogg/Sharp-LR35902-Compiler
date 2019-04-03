using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class IncrementNode : Node {
		public string VariableName { get; }

		public IncrementNode(string variablename) { VariableName = variablename; }

		public override IEnumerable<string> GetUsedRegisterNames() { yield return VariableName; }

		public override bool Equals(object obj) {
			if (obj is IncrementNode other)
				return other.VariableName == VariableName;

			return false;
		}

		public override IEnumerable<Node> GetChildren() => NoChildren;
	}
}
