using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class VariableValueNode : Node
    {
		public string VariableName { get; }

		public VariableValueNode(string name)
		{
			VariableName = name;
		}

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            yield return VariableName;
        }

		public override bool Equals(object obj)
		{
			if (obj is VariableValueNode other)
				return other.VariableName == VariableName;

			return false;
		}
	}
}
