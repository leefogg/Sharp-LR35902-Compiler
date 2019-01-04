﻿using System;
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

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            yield return VariableName;

            foreach (var usedvariable in Value.GetUsedRegisterNames())
                yield return usedvariable;
        }
    }
}
