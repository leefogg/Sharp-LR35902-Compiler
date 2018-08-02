using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class VariableValueNode : ValueNode
    {
		public string Name { get; }

		public VariableValueNode(string name)
		{
			Name = name;
		}
    }
}
