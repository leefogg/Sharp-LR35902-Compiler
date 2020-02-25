using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public class CompilationWarningException : WarningException
	{
		public byte[] CompiledInstruction { get; set; }

		public CompilationWarningException(byte[] assembledInstruction, string message, Exception innerException = null, string source = null) : base(message, innerException, source)
		{
			CompiledInstruction = assembledInstruction;
		}
	}
}
