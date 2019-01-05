using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class VariableValueNode : ValueNode
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
			if (obj is VariableValueNode)
			{
				var other = obj as VariableValueNode;
				return other.VariableName == VariableName;
			}

			return false;
		}
	}
}
