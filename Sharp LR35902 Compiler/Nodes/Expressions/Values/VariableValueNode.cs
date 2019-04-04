using System;
using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class VariableValueNode : ValueNode {
		public string VariableName { get; }

		public VariableValueNode(string name) { VariableName = name; }

		public override bool Matches(Node obj) {
			if (obj is VariableValueNode other)
				return other.VariableName == VariableName;

			return false;
		}

		public override IEnumerable<string> GetReadVariables() { yield return VariableName; }
		public override IEnumerable<Node> GetChildren() => NoChildren;

		public override ushort GetValue() { throw new NotSupportedException("This is a runtime feature only."); }

		public override ExpressionNode Optimize(IDictionary<string, ushort> knownvariables) {
			if (knownvariables.ContainsKey(VariableName))
				return new ShortValueNode(knownvariables[VariableName]);
			return this;
		}

		public override string ToString() => VariableName;
	}
}
