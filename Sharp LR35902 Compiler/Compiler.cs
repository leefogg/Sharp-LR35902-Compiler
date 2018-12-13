using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler
{
    public class Compiler
	{
		public static IDictionary<string, int> AllocateRegisters(Node astroot)
		{
			var start = NaiveAllocate(astroot);
			// TODO: Find last usage of each register and reuse in a loop
			return start;
		}

		public static IDictionary<string, int> NaiveAllocate(Node astroot)
		{
			var variabletoregister = new Dictionary<string, int>();
			var currentnode = 0;

			foreach (var node in astroot.GetChildren()) {
				if (node is VariableDeclarationNode dec)
					variabletoregister[dec.Name] = currentnode++;
			}

			return variabletoregister;
		}
	}
}
