using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
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
			rootnode.AddChild(new VariableAssignmentNode(expectedvariables[0], new VariableValueNode(expectedvariables[1]))); // x = y
			rootnode.AddChild(new VariableDeclarationNode("int", expectedvariables[2])); // int y

			var alloc = NaiveAllocate(rootnode);
			Assert.AreEqual(3, alloc.Keys.Count);

			var i = 0;
			foreach (var variable in alloc.Keys)
				Assert.AreEqual(expectedvariables[i++], variable);
			i = 0;
			foreach (var register in alloc.Values)
				Assert.AreEqual(register, i++);
		}
	}
}
