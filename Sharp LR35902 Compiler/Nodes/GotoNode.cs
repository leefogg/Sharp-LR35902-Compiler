using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class GotoNode : Node
	{
		public string LabelName { get; }

		public GotoNode(string labelName)
		{
			LabelName = labelName;
		}

		public override IEnumerable<string> GetUsedRegisterNames()
		{
			return NoRegisters;
		}

		public override bool Equals(object obj)
		{
			if (obj is GotoNode other)
				return other.LabelName == LabelName;

			return false;
		}
	}
}
