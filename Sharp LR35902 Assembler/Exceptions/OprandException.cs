using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.Exceptions
{
	public class OprandException : CompilationErrorException
	{
		public OprandException(string message, Exception innerException = null, string source = null) : base(message, innerException, source) { }
	}
}
