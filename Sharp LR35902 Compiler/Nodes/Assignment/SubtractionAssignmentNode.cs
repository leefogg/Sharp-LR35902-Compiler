namespace Sharp_LR35902_Compiler.Nodes {
	public class SubtractionAssignmentNode : VariableAssignmentNode {
		public SubtractionAssignmentNode(string variableName, ExpressionNode value) : base(variableName, value) { }

		public override bool Matches(Node obj) {
			if (obj is SubtractionAssignmentNode other)
				return other.VariableName == VariableName && other.Value.Matches(Value);

			return false;
		}
	}
}
