﻿using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class DecrementNode : Node {
		public string VariableName { get; }

		public DecrementNode(string variablename) { VariableName = variablename; }

		public override IEnumerable<string> GetUsedRegisterNames() { yield return VariableName; }

		public override bool Matches(Node obj) {
			if (obj is DecrementNode other)
				return other.VariableName == VariableName;

			return false;
		}

		public override IEnumerable<Node> GetChildren() => NoChildren;
	}
}
