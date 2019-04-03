using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class LabelNode : Node {
		public string Name { get; }

		public LabelNode(string name) { Name = name; }

		public override IEnumerable<string> GetWrittenVaraibles() => NoVariables;
		public override IEnumerable<string> GetReadVariables() => NoVariables;

		public override bool Matches(Node obj) {
			if (obj is LabelNode other)
				return other.Name == Name;

			return false;
		}

		public override IEnumerable<Node> GetChildren() => NoChildren;
	}
}
