using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class VariableAssignmentNode : Node
    {
		public string VariableName { get; }
		public ValueNode Value { get; }

		public VariableAssignmentNode(string variablename, ValueNode value)
		{
			VariableName = variablename;
			Value = value;
		}
    }
}
