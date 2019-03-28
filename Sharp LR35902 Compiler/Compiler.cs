using System;
using System.Collections.Generic;
using System.Linq;
using Common.Exceptions;
using Sharp_LR35902_Compiler.Nodes;

namespace Sharp_LR35902_Compiler {
	public class Compiler {
		public struct VariableUseRange {
			public string Name;
			public int Start, End;

			public VariableUseRange(string name, int start, int end) {
				Name = name;
				Start = start;
				End = end;
			}
		}

		public static IEnumerable<string> EmitAssembly(Node astroot) {
			char[] registernames = {'C', 'D', 'E', 'H', 'L'};

			var variablealloc = AllocateRegisters(astroot);

			// Check variable count is under the register limit
			if (variablealloc.Count > 0) // .Max throws if collection is empty :/
				if (variablealloc.Max(pair => pair.Value) >= registernames.Length)
					throw new OutOfSpaceException("Unable to allocate sufficent registers for optimized variable count");

			char GetVariableRegister(string name) => registernames[variablealloc[name]];

			string getValue(Node node) { // TODO: Make this ValueNode
				if (node is VariableValueNode assignment)
					return GetVariableRegister(assignment.VariableName).ToString();
				if (node is ShortValueNode shortValue)
					return shortValue.Value.ToString();

				throw new NotSupportedException("This operator is not yet supported inside the assembler. Please implement.");
			}

			// Cases must be in order of any inheritence because of the way `is` works
			var i = 1;
			foreach (var node in astroot.GetChildren())
				switch (node) {
					case VariableDeclarationNode _:
						// Do nothing
						break;
					case IncrementNode inc:
						yield return $"INC {GetVariableRegister(inc.VariableName)}";
						break;
					case DecrementNode dec:
						yield return $"DEC {GetVariableRegister(dec.VariableName)}";
						break;
					case LabelNode label:
						yield return label.Name + ':';
						break;
					case GotoNode gotonode:
						yield return "JP " + gotonode.LabelName;
						break;
					case VariableAssignmentNode var when var.Value is VariableValueNode varval:
						yield return $"LD {GetVariableRegister(var.VariableName)}, {GetVariableRegister(varval.VariableName)}";
						break;
					case VariableAssignmentNode var when var.Value is ShortValueNode imval:
						yield return $"LD {GetVariableRegister(var.VariableName)}, {imval.Value}";
						break;
					case VariableAssignmentNode var: {
						if (var.Value is OperatorNode oprator) {
							if (var.Value is ComparisonNode comparison) {
								yield return $"LD A {getValue(comparison.Left)}";
								yield return $"CP {getValue(comparison.Right)}";
								var skiplabelname = "generatedLabel" + i++;
								if (var.Value is LessThanComparisonNode) {
									yield return "JP NC " + skiplabelname;
								} else if (var.Value is MoreThanComparisonNode) {
									yield return "JP C " + skiplabelname;
									yield return "JP NZ " + skiplabelname;
								}

								yield return $"LD {GetVariableRegister(var.VariableName)} 1";
								yield return skiplabelname + ':';
							} else { // Only other operators should be addition and subtraction
								yield return $"LD B {getValue(oprator.Left)}";
								yield return $"LD A {getValue(oprator.Right)}";
								if (var.Value is AdditionNode)
									yield return "ADD A B";
								else if (var.Value is SubtractionNode)
									yield return "SUB A B";

								yield return $"LD {GetVariableRegister(var.VariableName)} A";
							} // TODO: Support negate operator
						}

						break;
					}
				}
		}

		public static IDictionary<string, int> AllocateRegisters(Node astroot) {
			var lifetimes = FindAllLastUsages(astroot);

			return OptimizeAllocation(lifetimes);
		}

		public static IDictionary<string, int> NaiveAllocate(Node astroot) {
			var variabletoregister = new Dictionary<string, int>();
			var currentnode = 0;

			foreach (var node in astroot.GetChildren())
				if (node is VariableDeclarationNode dec)
					variabletoregister[dec.VariableName] = currentnode++;

			return variabletoregister;
		}

		public static IDictionary<string, int> OptimizeAllocation(IEnumerable<VariableUseRange> variablelifetimes) {
			var allocations = new Dictionary<string, int>();
			var unallocatedvariables = variablelifetimes.ToList();

			var registerlifetimes = new int[unallocatedvariables.Count];

			var currentregister = 0;
			while (unallocatedvariables.Any()) {
				for (var i = 0; i < unallocatedvariables.Count; i++) {
					var variable = unallocatedvariables[i];
					if (registerlifetimes[currentregister] > variable.Start)
						continue;

					allocations[variable.Name] = currentregister;
					registerlifetimes[currentregister] = variable.End;
					unallocatedvariables.RemoveAt(i);
					i--;

					// Register start positions always increase. Will never find a variable assigned before registerlifetime.Start
				}

				currentregister++;
			}

			return allocations;
		}

		public static IEnumerable<VariableUseRange> FindAllLastUsages(Node rootnode) {
			var index = 0;
			var children = rootnode.GetChildren();

			var seenvariables = new List<string>();

			foreach (var node in children) {
				if (node is VariableAssignmentNode assignmentnode) {
					if (seenvariables.Contains(assignmentnode.VariableName))
						continue;
					var lastusage = FindLastVariableUsage(children, assignmentnode.VariableName, index);
					yield return new VariableUseRange(assignmentnode.VariableName, index, lastusage);
					seenvariables.Add(assignmentnode.VariableName);
				}

				index++;
			}
		}

		public static int FindLastVariableUsage(Node[] children, string variablename, int start = 0) {
			var lastusage = start;

			for (var i = start + 1; i < children.Length; i++) {
				var node = children[i];

				if (node is VariableDeclarationNode)
					continue;

				var usedvariables = node.GetUsedRegisterNames().Distinct();
				foreach (var usedvar in usedvariables)
					if (usedvar == variablename)
						lastusage = i;
			}

			return lastusage;
		}
	}
}
