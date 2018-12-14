using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Sharp_LR35902_Compiler.Compiler;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Compiler
	{
		[TestMethod]
		public void BasicRegisterAlloc()
		{
			var expectedvariables = new[] { "x", "y", "z" };
			var rootnode = new Node();
			rootnode.AddChild(new VariableDeclarationNode("int", expectedvariables[0])); // int x
			rootnode.AddChild(new VariableAssignmentNode(expectedvariables[0], new ImmediateValueNode(5))); // x = 5
			rootnode.AddChild(new VariableDeclarationNode("int", expectedvariables[1])); // int y
			rootnode.AddChild(new VariableAssignmentNode(expectedvariables[1], new VariableValueNode(expectedvariables[0]))); // y = x
			rootnode.AddChild(new VariableDeclarationNode("int", expectedvariables[2])); // int z

			var alloc = NaiveAllocate(rootnode);
			Assert.AreEqual(3, alloc.Keys.Count);

			var i = 0;
			foreach (var variable in alloc.Keys)
				Assert.AreEqual(expectedvariables[i++], variable);
			i = 0;
			foreach (var register in alloc.Values)
				Assert.AreEqual(register, i++);
		}

		[TestMethod]
		public void FindsLastVariableUsage()
		{
			var rootnode = new Node();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ImmediateValueNode(5))); // x = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "y")); // int y
			rootnode.AddChild(new VariableAssignmentNode("y", new VariableValueNode("x"))); // y = x
			rootnode.AddChild(new VariableDeclarationNode("int", "z")); // int z

			var lastusage = FindLastVariableUsage(rootnode, 0, "x");
			Assert.AreEqual(3, lastusage);
		}

		[TestMethod]
		public void FindsAllLastUses()
		{
			var rootnode = new Node();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ImmediateValueNode(5))); // x = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "y")); // int y
			rootnode.AddChild(new VariableAssignmentNode("y", new VariableValueNode("x"))); // y = x
			rootnode.AddChild(new VariableDeclarationNode("int", "z")); // int z

			var lastuses = FindAllLastUsages(rootnode).ToArray();
			Assert.AreEqual(3, lastuses.Length, "Not all variables found");
			Assert.AreEqual("x", lastuses[0].Name);
			Assert.AreEqual(0, lastuses[0].Start);
			Assert.AreEqual(3, lastuses[0].End);
			Assert.AreEqual("y", lastuses[1].Name);
			Assert.AreEqual(2, lastuses[1].Start);
			Assert.AreEqual(3, lastuses[1].End);
			Assert.AreEqual("z", lastuses[2].Name);
			Assert.AreEqual(4, lastuses[2].Start);
			Assert.AreEqual(4, lastuses[2].End);
		}

		[TestMethod]
		public void ReallocatesRegisters_Simple()
		{
			var timelines = new[]
			{
				new VariableUseRage("x", 0,1), // int x = 1
				new VariableUseRage("y", 2,3), // int y = 1
				new VariableUseRage("z", 4,5), // int z = 1
			};

			var newallocations = OptimizeAllocation(timelines);
			Assert.AreEqual(3, newallocations.Keys.Count);
			Assert.AreEqual(0, newallocations["x"]);
			Assert.AreEqual(0, newallocations["y"]);
			Assert.AreEqual(0, newallocations["z"]);
		}

		[TestMethod]
		public void ReallocatesRegisters_Overlap()
		{
			var timelines = new[]
			{
				new VariableUseRage("x", 0,5), 
				new VariableUseRage("y", 2,3), 
				new VariableUseRage("z", 4,5),
			};

			var newallocations = OptimizeAllocation(timelines);
			Assert.AreEqual(3, newallocations.Keys.Count);
			Assert.AreEqual(0, newallocations["x"]);
			Assert.AreEqual(1, newallocations["y"]);
			Assert.AreEqual(1, newallocations["z"]);
		}
	}
}
