using Common.Exceptions;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharp_LR35902_Compiler
{
    public class Compiler
	{
		public struct VariableUseRage
		{
			public string Name;
			public int Start, End;

			public VariableUseRage(string name, int start, int end)
			{
				Name = name;
				Start = start;
				End = end;
			}
		}

		public static IEnumerable<string> EmitAssembly(Node astroot)
		{
			char[] registernames = new[] { 'C', 'D', 'E', 'H', 'L' };

			var variablealloc = AllocateRegisters(astroot);

			// Check variable count is under the register limit
			if (variablealloc.Count > 0) // .Max throws if collection is empty :/
				if (variablealloc.Max(pair => pair.Value) >= registernames.Length)
					throw new OutOfSpaceException("Unable to allocate sufficent registers for optimized variable count");

			char getVariableRegister(string name) =>
				registernames[variablealloc[name]];

			string writeToRegister(ExpressionNode value, string outputreg)
			{
				if (value is VariableValueNode vval)
					return $"LD {outputreg}, {getVariableRegister(vval.VariableName)}";
				else if (value is ShortValueNode cval)
					return $"LD {outputreg}, {cval.Value}";

				throw new NotSupportedException("This operator is not yet supported inside the assembler. Please implement.");
			}

			// Cases must be in order of any inheritence because of the way `is` works
			var i = 1;
			foreach (var node in astroot.GetChildren())
			{
				if (node is VariableDeclarationNode)
				{
					// Do nothing
				} else if (node is IncrementNode inc) {
					yield return $"INC {getVariableRegister(inc.VariableName)}";
				} else if (node is DecrementNode dec) {
					yield return $"DEC {getVariableRegister(dec.VariableName)}";
				} else if (node is LabelNode label)	{
					yield return label.Name + ':';
				} else if (node is GotoNode gotonode) {
					yield return "JP " + gotonode.LabelName;
				} else if (node is VariableAssignmentNode var) {
					if (var.Value is VariableValueNode varval)
						yield return $"LD {getVariableRegister(var.VariableName)}, {getVariableRegister(varval.VariableName)}";
					else if (var.Value is ShortValueNode imval)
						yield return $"LD {getVariableRegister(var.VariableName)}, {imval.Value}";
					else if (var.Value is OperatorNode oprator)
					{
						if (var.Value is ComparisonNode comparison)
						{
							yield return writeToRegister(comparison.Left, "A");
							yield return writeToRegister(comparison.Right, "B");
							yield return "CP B";
							var skiplabelname = "generatedLabel" + i++;
							if (var.Value is LessThanComparisonNode)
							{
								yield return "JP NC " + skiplabelname;
							} 
							else if (var.Value is MoreThanComparisonNode)
							{
								yield return "JP C " + skiplabelname;
								yield return "JP NZ " + skiplabelname;
							}
							yield return $"LD {getVariableRegister(var.VariableName)} 1";
							yield return skiplabelname + ':';
						}
						else
						{
							yield return writeToRegister(oprator.Left, "B");
							yield return writeToRegister(oprator.Right, "A");
							if (var.Value is AdditionNode)
								yield return "ADD A, B";
							else if (var.Value is SubtractionNode)
								yield return "SUB A, B";
							yield return $"LD {getVariableRegister(var.VariableName)}, A";
						}
					}
				} 
			}
		}

		public static IDictionary<string, int> AllocateRegisters(Node astroot)
		{
			var lifetimes = FindAllLastUsages(astroot);

			return OptimizeAllocation(lifetimes);
		}

		public static IDictionary<string, int> NaiveAllocate(Node astroot)
		{
			var variabletoregister = new Dictionary<string, int>();
			var currentnode = 0;

			foreach (var node in astroot.GetChildren()) {
				if (node is VariableDeclarationNode dec)
					variabletoregister[dec.VariableName] = currentnode++;
			}

			return variabletoregister;
		}

		public static IDictionary<string, int> OptimizeAllocation(IEnumerable<VariableUseRage> variablelifetimes)
		{
			var allocations = new Dictionary<string, int>();
			var unallocatedvariables = variablelifetimes.ToList();

			var registerlifetimes = new int[unallocatedvariables.Count];

			var currentregister = 0;
			while (unallocatedvariables.Any())
			{
				for (var i=0; i<unallocatedvariables.Count; i++)
				{
					var variable = unallocatedvariables[i];
					if (registerlifetimes[currentregister] <= variable.Start)
					{
						allocations[variable.Name] = currentregister;
						registerlifetimes[currentregister] = variable.End;
						unallocatedvariables.RemoveAt(i);
						i--;
					}
					// Register start positions always increase. Will never find a variable assigned before registerlifetime.Start
				}

				currentregister++;
			}

			return allocations;
		}

		public static IEnumerable<VariableUseRage> FindAllLastUsages(Node rootnode)
		{
			var lastuses = new Dictionary<string, VariableUseRage>();

			var index = 0;
			foreach (var node in rootnode.GetChildren())
			{
				if (node is VariableDeclarationNode dec)
				{
					var lastusage = FindLastVariableUsage(rootnode, index, dec.VariableName);
					yield return new VariableUseRage(dec.VariableName, index, lastusage);
				}

				index++;
			}
		}

		public static int FindLastVariableUsage(Node rootnote, int start, string variablename)
		{
			int lastusage = start;

			var children = rootnote.GetChildren();
			for (var i=start+1; i<children.Length; i++)
			{
				var node = children[i];
				
                if (!(node is VariableDeclarationNode))
                {
                    var usedvariables = node.GetUsedRegisterNames().Distinct();
                    foreach (var usedvar in usedvariables)
                        if (usedvar == variablename)
                            lastusage = i;
                }
			}

			return lastusage;
		}
	}
}
