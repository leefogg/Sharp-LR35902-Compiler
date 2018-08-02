using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class DecrementNode : Node
	{
		public string VariableName { get; }

		public DecrementNode(string variablename)
		{
			VariableName = variablename;
		}
	}
}