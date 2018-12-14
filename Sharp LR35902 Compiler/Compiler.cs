﻿using Sharp_LR35902_Compiler.Nodes;
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
					var lastusage = FindLastVariableUsage(rootnode, index, dec.Name);
					yield return new VariableUseRage(dec.Name, index, lastusage);
				}

				index++;
			}
		}

		public static int FindLastVariableUsage(Node rootnote, int start, string variablename)
		{
			int lastuseage = start;

			var children = rootnote.GetChildren();
			for (var i=start+1; i<children.Length; i++)
			{
				var node = children[i];
				if (node is VariableAssignmentNode assignment)
				{
					if (assignment.VariableName == variablename)
						lastuseage = i;
					if (assignment.Value is VariableValueNode val)
						if (val.Name == variablename)
							lastuseage = i;
				}
			}

			return lastuseage;
		}
	}
}