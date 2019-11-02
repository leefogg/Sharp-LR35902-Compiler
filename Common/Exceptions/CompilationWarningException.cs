using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public class CompilationWarningException : WarningException
	{
		public byte[] CompiledInstruction { get; set; }

		public CompilationWarningException(byte[] assembledInstruction, string message) : base(message)
		{
			CompiledInstruction = assembledInstruction;
		}
		public CompilationWarningException(byte[] assembledInstruction, string message, Exception innerException) : base(message, innerException)
		{
			CompiledInstruction = assembledInstruction;
		}
	}
}
