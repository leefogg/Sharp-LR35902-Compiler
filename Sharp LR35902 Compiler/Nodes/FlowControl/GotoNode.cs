using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class GotoNode : Node {
		public string LabelName { get; }

		public GotoNode(string labelName) { LabelName = labelName; }

		public override IEnumerable<string> GetWrittenVaraibles() => NoVariables;
		public override IEnumerable<string> GetReadVariables() => NoVariables;

		public override bool Matches(Node obj) {
			if (obj is GotoNode other)
				return other.LabelName == LabelName;

			return false;
		}

		public override IEnumerable<Node> GetChildren() => NoChildren;
	}
}
