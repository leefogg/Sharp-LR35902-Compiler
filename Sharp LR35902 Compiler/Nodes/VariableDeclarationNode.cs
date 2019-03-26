using System.Collections.Generic;

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

		public override bool Equals(object obj)
		{
			if (obj is VariableDeclarationNode other)
				return other.VariableName == VariableName && other.DataType == DataType;

			return false;
		}

		public override Node[] GetChildren() => NoChildren;
	}
}
