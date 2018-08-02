using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class IncrementNode : Node
    {
		public string VariableName { get; }

		public IncrementNode(string variablename)
		{
			VariableName = variablename;
		}
	}
}
