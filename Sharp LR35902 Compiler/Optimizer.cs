using Sharp_LR35902_Compiler.Nodes;
using System.Collections.Generic;

namespace Sharp_LR35902_Compiler
{
	public static class Optimizer
	{
		public static bool PropagateConstants(BlockNode block)
		{
			var changesmade = false;

			var variablevalues = new Dictionary<string, ushort>();
			var children = block.GetChildren();

			for (var i=0; i<children.Length; i++)
			{
				var node = children[i];

				if (node is VariableAssignmentNode assignment)
				{
					if (assignment.Value is ValueNode value)
						if (variablevalues.ContainsKey(assignment.VariableName))
							variablevalues[assignment.VariableName] = value.GetValue();
						else
							variablevalues.Add(assignment.VariableName, value.GetValue());
					else if (assignment.Value is VariableValueNode varval)
					{
						if (variablevalues.ContainsKey(varval.VariableName))
						{
							assignment.Value = new ShortValueNode(variablevalues[varval.VariableName]);
							changesmade = true;
							i--; // We know this is a valuenode now so go to top to add it to known values
						}
					}
				}
			}

			return changesmade;
		}

		public static bool RemoveUnusedVariables(BlockNode block)
		{
			// Dictionary should be safe as parser checks for redeclarations
			var usedvariables = new Dictionary<string, bool>();

			var children = block.GetChildren();

			// Scan for variables and uses
			foreach (var node in children)
			{
				if (node is VariableDeclarationNode dec)
					usedvariables.Add(dec.VariableName, false);

				// Check for reads, writes aren't a "use"
				if (node is VariableAssignmentNode assignment)
					foreach (var usedvar in assignment.Value.GetUsedRegisterNames())
						usedvariables[usedvar] = true; // Should be safe as parser checks for variable existence before assignment
			}

			var changesmade = false;
			var i = 0;
			// Remove
			foreach (var node in children)
			{
				if (node is VariableDeclarationNode dec) { 
					if (usedvariables[dec.VariableName] == false)
					{
						block.RemoveChild(i--);
						changesmade = true;
					}
				} 
				else if (node is VariableAssignmentNode assignment) {
					if (usedvariables[assignment.VariableName] == false)
					{
						block.RemoveChild(i--);
						changesmade = true;
					}
				}

				i++;
			}

			return changesmade;
		}
	}
}
