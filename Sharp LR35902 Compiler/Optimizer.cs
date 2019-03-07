using Sharp_LR35902_Compiler.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler
{
	public static class Optimizer
	{
		public static void Optimize(BlockNode block)
		{
			bool changesmade;
			do
			{
				changesmade = false;

				if (PropagateConstants(block))
					changesmade = true;

				if (RemoveUnusedVariables(block))
					changesmade = true;

			} while (changesmade);
		}

		public static bool PropagateConstants(BlockNode block)
		{
			var changesmade = false;

			var variablevalues = new Dictionary<string, ushort>();
			var children = block.GetChildren().ToList();

			for (var i=0; i<children.Count; i++)
			{
				var node = children[i];

				if (node is VariableAssignmentNode assignment)
				{
					var newvalue = assignment.Value.Optimize(variablevalues);
					if (assignment.Value != newvalue)
						changesmade = true;
					assignment.Value = newvalue;

					if (assignment.Value is ConstantNode value)
					{
						if (variablevalues.ContainsKey(assignment.VariableName))
							variablevalues[assignment.VariableName] = value.GetValue();
						else
							variablevalues.Add(assignment.VariableName, value.GetValue());
					}
				} 
				else if (node is IncrementNode inc)
				{
					variablevalues[inc.VariableName]++;
					children.RemoveAt(i);
					block.RemoveChild(i--);
					changesmade = true;
				}
				else if (node is DecrementNode dec)
				{
					variablevalues[dec.VariableName]--;
					children.RemoveAt(i);
					block.RemoveChild(i--);
					changesmade = true;
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
