using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.Exceptions
{
	public class OprandException : CompilationErrorException
	{
		public OprandException(string message) : base(message) { }
		public OprandException(string message, Exception innerException) : base(message, innerException) { }
	}
}
