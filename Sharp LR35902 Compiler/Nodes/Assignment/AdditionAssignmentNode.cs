namespace Sharp_LR35902_Compiler.Nodes {
	public class AdditionAssignmentNode : VariableAssignmentNode {
		public AdditionAssignmentNode(string variableName, ExpressionNode value) : base(variableName, value) { }

		public override bool Equals(object obj) {
			if (obj is AdditionAssignmentNode other)
				return other.VariableName == VariableName && other.Value.Equals(Value);

			return false;
		}
	}
}
