using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class VariableDeclarationNode : Node {
		public string DataType { get; }
		public string VariableName { get; }

		public VariableDeclarationNode(string datatype, string name) {
			DataType = datatype;
			VariableName = name;
		}

		public override IEnumerable<string> GetWrittenVaraibles() => NoVariables;
		public override IEnumerable<string> GetReadVariables() => NoVariables;
		

		public override bool Matches(Node obj) {
			if (obj is VariableDeclarationNode other)
				return other.VariableName == VariableName && other.DataType == DataType;

			return false;
		}

		public override IEnumerable<Node> GetChildren() => NoChildren;

		public override string ToString() => DataType + ' ' + VariableName;
	}
}
