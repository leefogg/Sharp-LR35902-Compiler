using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.Exceptions
{
	public class InvalidRangeExcpetion : CompilationWarningException
	{
		public InvalidRangeExcpetion(byte[] assembledInstruction, string message, Exception innerException = null, string source = null) : base(assembledInstruction, message, innerException, source) { }
	}
}
