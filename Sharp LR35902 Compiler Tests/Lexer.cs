using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Sharp_LR35902_Compiler.Lexer;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Lexer
	{
		[TestMethod]
		public void MyTestMethod()
		{
			var strings = CreateAST("for (int i = 0; i != 99; i++) {");
		}
	}
}
