using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class VariableDeclarationNode : Node
	{
		public string DataType { get; }
		public string VariableName { get; }

		public VariableDeclarationNode(string datatype, string name)
		{
			DataType = datatype;
			VariableName = name;
		}

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            yield return VariableName;
        }
    }
}
