using System.Collections.Generic;
using System.Linq;
using Common.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler.Nodes;
using static Test_Common.Utils;
using static Sharp_LR35902_Compiler.Compiler;

namespace Sharp_LR35902_Compiler_Tests {
	[TestClass]
	public class Compiler {
		[TestMethod]
		public void BasicRegisterAlloc() {
			var expectedvariables = new[] {"x", "y", "z"};
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", expectedvariables[0])); // int x
			rootnode.AddChild(new VariableAssignmentNode(expectedvariables[0], new ShortValueNode(5))); // x = 5
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
		public void FindLastUsage_Finds() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5))); // x = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "y")); // int y
			rootnode.AddChild(new VariableAssignmentNode("y", new VariableValueNode("x"))); // y = x
			rootnode.AddChild(new VariableDeclarationNode("int", "z")); // int z

			var lastusage = FindLastVariableUsage(rootnode.GetChildren(), "x", 1);
			Assert.AreEqual(3, lastusage);
		}

		[TestMethod]
		public void FindLastUsage_DoesntFind() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5))); // x = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "y")); // int y
			rootnode.AddChild(new VariableAssignmentNode("y", new VariableValueNode("x"))); // y = x
			rootnode.AddChild(new VariableDeclarationNode("int", "z")); // int z

			var lastusage = FindLastVariableUsage(rootnode.GetChildren(), "x", 10);
			Assert.AreEqual(10, lastusage);
		}

		[TestMethod]
		public void FindsAllLastUses() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5))); // x = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "y")); // int y
			rootnode.AddChild(new VariableAssignmentNode("y", new VariableValueNode("x"))); // y = x
			rootnode.AddChild(new VariableDeclarationNode("int", "z")); // int z
			rootnode.AddChild(new VariableAssignmentNode("y", new ShortValueNode(10))); // y = 10

			var lastuses = FindAllLastUsages(rootnode).ToArray();
			Assert.AreEqual(2, lastuses.Length, "Not all variables found");
			Assert.AreEqual("x", lastuses[0].Name);
			Assert.AreEqual(1, lastuses[0].Start);
			Assert.AreEqual(3, lastuses[0].End);
			Assert.AreEqual("y", lastuses[1].Name);
			Assert.AreEqual(3, lastuses[1].Start);
			Assert.AreEqual(5, lastuses[1].End);
		}

		[TestMethod]
		public void ReallocatesRegisters_Simple() {
			var timelines = new[] {
				new VariableUseRange("x", 0, 1), // int x = 1
				new VariableUseRange("y", 2, 3), // int y = 1
				new VariableUseRange("z", 4, 5) // int z = 1
			};

			var newallocations = OptimizeAllocation(timelines);
			Assert.AreEqual(3, newallocations.Keys.Count);
			Assert.AreEqual(0, newallocations["x"]);
			Assert.AreEqual(0, newallocations["y"]);
			Assert.AreEqual(0, newallocations["z"]);
		}

		[TestMethod]
		public void ReallocatesRegisters_Overlap() {
			var timelines = new[] {
				new VariableUseRange("x", 0, 5),
				new VariableUseRange("y", 2, 3),
				new VariableUseRange("z", 4, 5)
			};

			var newallocations = OptimizeAllocation(timelines);
			Assert.AreEqual(3, newallocations.Keys.Count);
			Assert.AreEqual(0, newallocations["x"]);
			Assert.AreEqual(1, newallocations["y"]);
			Assert.AreEqual(1, newallocations["z"]);
		}

		[TestMethod]
		public void EmitAssembly_LoadValueToRegister() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5))); // x = 5

			var asmlines = new List<string>(EmitAssembly(rootnode));
			Assert.AreEqual(1, asmlines.Count);
			Assert.AreEqual("LD C 5", asmlines[0]);
		}

		[TestMethod]
		public void EmitAssembly_LoadRegisterToRegister() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5))); // x = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "y")); // int y
			rootnode.AddChild(new VariableAssignmentNode("y", new VariableValueNode("x"))); // y = x

			var asmlines = new List<string>(EmitAssembly(rootnode));
			Assert.AreEqual(2, asmlines.Count);
			Assert.AreEqual("LD C 5", asmlines[0]);
			Assert.AreEqual("LD C C", asmlines[1]); // x's last use is to create y so C gets reused
		}

		[TestMethod]
		public void EmitAssembly_IncrementVariable() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5))); // x = 5
			rootnode.AddChild(new IncrementNode("x")); // x++

			var asmlines = new List<string>(EmitAssembly(rootnode));
			Assert.AreEqual(2, asmlines.Count);
			Assert.AreEqual("LD C 5", asmlines[0]);
			Assert.AreEqual("INC C", asmlines[1]);
		}

		[TestMethod]
		public void EmitAssembly_DecrementVariable() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "x")); // int x
			rootnode.AddChild(new VariableAssignmentNode("x", new ShortValueNode(5))); // x = 5
			rootnode.AddChild(new DecrementNode("x")); // x++

			var asmlines = new List<string>(EmitAssembly(rootnode));
			Assert.AreEqual(2, asmlines.Count);
			Assert.AreEqual("LD C 5", asmlines[0]);
			Assert.AreEqual("DEC C", asmlines[1]);
		}

		[TestMethod]
		public void EmitAssembly_Blank() {
			var rootnode = new ASTNode();

			var asmlines = new List<string>(EmitAssembly(rootnode));

			Assert.AreEqual(0, asmlines.Count);
		}

		[TestMethod]
		public void EmitAssembly_Label() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new LabelNode("label")); // label:

			var asmlines = new List<string>(EmitAssembly(rootnode));

			Assert.AreEqual(1, asmlines.Count);
			Assert.AreEqual("label:", asmlines[0]);
		}

		[TestMethod]
		public void EmitAssembly_Goto() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new GotoNode("label")); // goto label

			var asmlines = new List<string>(EmitAssembly(rootnode));

			ListEqual(new List<string> {"JP label"}, asmlines);
		}

		[TestMethod]
		public void EmitAssembly_UsesAllRegisters() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "b")); // int b
			rootnode.AddChild(new VariableAssignmentNode("b", new ShortValueNode(5))); // b = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "c")); // int c
			rootnode.AddChild(new VariableAssignmentNode("c", new ShortValueNode(5))); // c = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "d")); // int d
			rootnode.AddChild(new VariableAssignmentNode("d", new ShortValueNode(5))); // d = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "e")); // int e
			rootnode.AddChild(new VariableAssignmentNode("e", new ShortValueNode(5))); // e = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "h")); // int h
			rootnode.AddChild(new VariableAssignmentNode("h", new ShortValueNode(5))); // h = 5
			rootnode.AddChild(new IncrementNode("b"));
			rootnode.AddChild(new IncrementNode("c"));
			rootnode.AddChild(new IncrementNode("d"));
			rootnode.AddChild(new IncrementNode("e"));
			rootnode.AddChild(new IncrementNode("h"));

			var asmlines = new List<string>(EmitAssembly(rootnode));
			Assert.AreEqual(10, asmlines.Count);
			Assert.AreEqual("LD C 5", asmlines[0]);
			Assert.AreEqual("LD D 5", asmlines[1]);
			Assert.AreEqual("LD E 5", asmlines[2]);
			Assert.AreEqual("LD H 5", asmlines[3]);
			Assert.AreEqual("LD L 5", asmlines[4]);
		}

		[TestMethod]
		[ExpectedException(typeof(OutOfSpaceException))]
		public void EmitAssembly_RegisterOverflowThrows() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("int", "b")); // int b
			rootnode.AddChild(new VariableAssignmentNode("b", new ShortValueNode(5))); // b = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "c")); // int c
			rootnode.AddChild(new VariableAssignmentNode("c", new ShortValueNode(5))); // c = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "d")); // int d
			rootnode.AddChild(new VariableAssignmentNode("d", new ShortValueNode(5))); // d = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "e")); // int e
			rootnode.AddChild(new VariableAssignmentNode("e", new ShortValueNode(5))); // e = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "h")); // int h
			rootnode.AddChild(new VariableAssignmentNode("h", new ShortValueNode(5))); // h = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "l")); // int l
			rootnode.AddChild(new VariableAssignmentNode("l", new ShortValueNode(5))); // l = 5
			rootnode.AddChild(new VariableDeclarationNode("int", "overflow")); // int overflow
			rootnode.AddChild(new VariableAssignmentNode("overflow", new ShortValueNode(5))); // overflow = 5
			rootnode.AddChild(new IncrementNode("b"));
			rootnode.AddChild(new IncrementNode("c"));
			rootnode.AddChild(new IncrementNode("d"));
			rootnode.AddChild(new IncrementNode("e"));
			rootnode.AddChild(new IncrementNode("h"));
			rootnode.AddChild(new IncrementNode("l"));
			rootnode.AddChild(new IncrementNode("overflow"));

			EmitAssembly(rootnode).ToArray(); // ToArray needed here to force iterator to complete yielding
		}


		[TestMethod]
		public void EmitAssembly_Expression_Addition() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("byte", "x"));
			rootnode.AddChild(new VariableAssignmentNode("x", new AdditionNode(new ShortValueNode(5), new ShortValueNode(6))));

			var actualASM = new List<string>(EmitAssembly(rootnode));

			var expectedASM = new[] {
				"LD B 5",
				"LD A 6",
				"ADD A B",
				"LD C A"
			};

			ListEqual(expectedASM, actualASM);
		}

		[TestMethod]
		public void EmitAssembly_Expression_Subtraction() {
			var rootnode = new ASTNode();
			rootnode.AddChild(new VariableDeclarationNode("byte", "x"));
			rootnode.AddChild(new VariableAssignmentNode("x", new SubtractionNode(new ShortValueNode(5), new ShortValueNode(6))));

			var actualASM = new List<string>(EmitAssembly(rootnode));

			var expectedASM = new[] {
				"LD B 5",
				"LD A 6",
				"SUB A B",
				"LD C A"
			};

			ListEqual(expectedASM, actualASM);
		}

		[TestMethod]
		public void EmitAssembly_Expression_LessThen() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x",
				new LessThanComparisonNode(
					new ShortValueNode(1),
					new ShortValueNode(2)
				)
			));

			var asm = EmitAssembly(ast).ToArray();

			ListEqual(new[] {
				"LD A 1",
				"CP 2",
				"JP NC generatedLabel1",
				"LD C 1",
				"JP generatedLabel2",
				"generatedLabel1:",
				"LD C 0",
				"generatedLabel2:"
			}, asm);
		}

		[TestMethod]
		public void EmitAssembly_Expression_MoreThen() {
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x",
				new MoreThanComparisonNode(
					new ShortValueNode(1),
					new ShortValueNode(2)
				)
			));

			var asm = EmitAssembly(ast).ToArray();

			ListEqual(new[] {
				"LD A 1",
				"CP 2",
				"JP C generatedLabel1",
				"JP NZ generatedLabel1",
				"LD C 1",
				"JP generatedLabel2",
				"generatedLabel1:",
				"LD C 0",
				"generatedLabel2:"
			}, asm);
		}

		[TestMethod]
		public void EmitAssembly_Expression_Negate()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new NegateNode(new ShortValueNode(1))));

			var asm = EmitAssembly(ast).ToArray();

			ListEqual(new[] {
				"LD A 1",
				"XOR 1",
				"LD C A"
			}, asm);
		}

		[TestMethod]
		public void EmitAssembly_If_CreatesBlock()
		{
			var ast = new ASTNode();
			ast.AddChild(new VariableDeclarationNode("byte", "x"));
			ast.AddChild(new VariableAssignmentNode("x", new ShortValueNode(1)));
			var block = new BlockNode();
			block.AddChild(new VariableAssignmentNode("x", new ShortValueNode(1)));
			ast.AddChild(new IfNode(new VariableValueNode("x"), block));

			var asm = EmitAssembly(ast).ToArray();

			ListEqual(new[] {
				"LD C 1",
				"CP C",
				"JP NZ generatedLabel1",
				"LD C 1",
				"generatedLabel1:"
			}, asm);
		}
	}
}
