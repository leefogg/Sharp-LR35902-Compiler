using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Scope
	{
		private static VariableMember TestMember = new VariableMember("int", "test");

		[TestMethod]
		public void FindsLocalMember()
		{
			var scope = new Sharp_LR35902_Compiler.Scope();
			scope.AddMember(TestMember);

			Assert.IsNotNull(scope.GetLocalMember("test"));
		}

		[TestMethod]
		public void FindsParentMember()
		{
			var parentscope = new Sharp_LR35902_Compiler.Scope();
			parentscope.AddMember(TestMember);
			var chiledscope = new Sharp_LR35902_Compiler.Scope(parentscope);

			Assert.IsNotNull(chiledscope.GetMember("test"));
		}
	}
}
