using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sharp_LR35902_Compiler.Exceptions;
using static Sharp_LR35902_Compiler.Assembler;

namespace Sharp_LR35902_Compiler_Tests
{
	[TestClass]
	public class Assembler
	{
		[TestMethod]
		[ExpectedException(typeof(NotFoundException))]
		public void UnrecognizedInstruction()
		{
			CompileInstruction("Something strange");
		}
	}
}
