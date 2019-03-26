using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Nodes
	{
		private static readonly Dictionary<string, ushort> NoVariables = new Dictionary<string, ushort>();

		[TestMethod]
		public void Variable_Optimize_ReplacesKnown()
		{
			var originalnode = new VariableValueNode("x");

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() { { "x", 3 } });

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void Variable_Optimize_KeepsUnknown()
		{
			var originalnode = new VariableValueNode("x");

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(VariableValueNode));
		}

		[TestMethod]
		public void Addition_Optimize_FullyConstantCollapses()
		{
			var originalnode = new AdditionNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}
		[TestMethod]
		public void Addition_Optimize_UnknownVariableRemains()
		{
			var originalnode = new AdditionNode(new ShortValueNode(1), new VariableValueNode("x"));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(AdditionNode));
		}

		[TestMethod]
		public void Subtraction_Optimize_FullyConstantCollapses()
		{
			var originalnode = new SubtractionNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}
		[TestMethod]
		public void Subtraction_Optimize_UnknownVariableRemains()
		{
			var originalnode = new SubtractionNode(new ShortValueNode(1), new VariableValueNode("x"));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(SubtractionNode));
		}

		[TestMethod]
		public void AndComparison_Optimize_FullyConstantCollapses()
		{
			var originalnode = new AndComparisonNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}
		[TestMethod]
		public void AndComparison_Optimize_UnknownVariableRemains()
		{
			var originalnode = new AndComparisonNode(new ShortValueNode(1), new VariableValueNode("x"));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(AndComparisonNode));
		}

		[TestMethod]
		public void OrComparison_Optimize_Lazy_AnyConstantCollapses_Left()
		{
			var originalnode = new OrComparisonNode(new ShortValueNode(1), new VariableValueNode("x"));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void OrComparison_Optimize_Lazy_AnyConstantCollapses_Right()
		{
			var originalnode = new OrComparisonNode(new VariableValueNode("x"), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void EqualsComparison_Optimize_FullyConstantCollapses()
		{
			var originalnode = new EqualsComparisonNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}
		[TestMethod]
		public void EqualsComparison_Optimize_UnknownVariableRemains()
		{
			var originalnode = new EqualsComparisonNode(new ShortValueNode(1), new VariableValueNode("x"));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(EqualsComparisonNode));
		}

		[TestMethod]
		public void MoreThanComparison_Optimize_FullyConstantCollapses()
		{
			var originalnode = new MoreThanComparisonNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}
		[TestMethod]
		public void MoreThanComparison_Optimize_UnknownVariableRemains()
		{
			var originalnode = new MoreThanComparisonNode(new ShortValueNode(1), new VariableValueNode("x"));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(MoreThanComparisonNode));
		}

		[TestMethod]
		public void LessThanComparison_Optimize_FullyConstantCollapses()
		{
			var originalnode = new LessThanComparisonNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}
		[TestMethod]
		public void LessThanComparison_Optimize_UnknownVariableRemains()
		{
			var originalnode = new LessThanComparisonNode(new ShortValueNode(1), new VariableValueNode("x"));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.IsInstanceOfType(optimizednode, typeof(LessThanComparisonNode));
		}

		[TestMethod]
		public void Addition_Optimize_RecursiveReduction()
		{
			var originalnode = new AdditionNode(
				new VariableValueNode("a"),
				new VariableValueNode("b")
			);

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 1 },
				{ "b", 1 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void Addition_Optimize_FullyConstant_Value()
		{
			var originalnode = new AdditionNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(2, optimizednode.GetValue());
		}

		[TestMethod]
		public void Subtraction_Optimize_RecursiveReduction()
		{
			var originalnode = new SubtractionNode(
				new VariableValueNode("a"),
				new VariableValueNode("b")
			);

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 1 },
				{ "b", 1 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void Subtraction_Optimize_FullyConstant_Value()
		{
			var originalnode = new SubtractionNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(0, optimizednode.GetValue());
		}

		[TestMethod]
		public void And_Optimize_RecursiveReduction()
		{
			var originalnode = new AndComparisonNode(
				new VariableValueNode("a"),
				new VariableValueNode("b")
			);

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 1 },
				{ "b", 1 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void And_Optimize_FullyConstant_Value()
		{
			var originalnode = new AndComparisonNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(1, optimizednode.GetValue());
		}

		[TestMethod]
		public void Or_Optimize_RecursiveReduction()
		{
			var originalnode = new OrComparisonNode(
				new VariableValueNode("a"),
				new VariableValueNode("b")
			);

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 0 },
				{ "b", 1 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void Or_Optimize_FullyConstant_Value()
		{
			var originalnode = new OrComparisonNode(new ShortValueNode(1), new ShortValueNode(0));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(1, optimizednode.GetValue());
		}

		[TestMethod]
		public void LessThan_Optimize_RecursiveReduction()
		{
			var originalnode = new LessThanComparisonNode(
				new VariableValueNode("a"),
				new VariableValueNode("b")
			);

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 0 },
				{ "b", 1 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void LessThan_Optimize_FullyConstant_Value()
		{
			var originalnode = new LessThanComparisonNode(new ShortValueNode(1), new ShortValueNode(2));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(1, optimizednode.GetValue());
		}

		[TestMethod]
		public void MoreThan_Optimize_RecursiveReduction()
		{
			var originalnode = new MoreThanComparisonNode(
				new VariableValueNode("a"),
				new VariableValueNode("b")
			);

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 0 },
				{ "b", 1 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void MoreThan_Optimize_FullyConstant_Value()
		{
			var originalnode = new MoreThanComparisonNode(new ShortValueNode(2), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(1, optimizednode.GetValue());
		}

		[TestMethod]
		public void Equal_Optimize_RecursiveReduction()
		{
			var originalnode = new EqualsComparisonNode(
				new VariableValueNode("a"),
				new VariableValueNode("b")
			);

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 0 },
				{ "b", 1 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void Equal_Optimize_FullyConstant_Value()
		{
			var originalnode = new EqualsComparisonNode(new ShortValueNode(1), new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(1, optimizednode.GetValue());
		}

		[TestMethod]
		public void Negate_Optimize_RecursiveReduction()
		{
			var originalnode = new NegateNode(new VariableValueNode("a"));

			var optimizednode = originalnode.Optimize(new Dictionary<string, ushort>() {
				{ "a", 0 }
			});

			Assert.IsInstanceOfType(optimizednode, typeof(ConstantNode));
		}

		[TestMethod]
		public void Negate_Optimize_FullyConstant_Value()
		{
			var originalnode = new NegateNode(new ShortValueNode(1));

			var optimizednode = originalnode.Optimize(NoVariables);

			Assert.AreEqual(0, optimizednode.GetValue());
		}
	}
}
