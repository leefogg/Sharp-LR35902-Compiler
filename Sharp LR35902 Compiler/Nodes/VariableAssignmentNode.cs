using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class VariableAssignmentNode : Node
    {
		public string VariableName { get; }
		public Node Value { get; }

		public VariableAssignmentNode(string variablename, Node value)
		{
			VariableName = variablename;
			Value = value;
		}

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            yield return VariableName;

            foreach (var usedvariable in Value.GetUsedRegisterNames())
                yield return usedvariable;
        }

		public override bool Equals(object obj)
		{
			if (obj is VariableAssignmentNode other)
				return other.VariableName == VariableName && other.Value.Equals(Value);

			return false;
		}
	}
}
