using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public class CompilationErrorException : ErrorException
	{
		public CompilationErrorException(string message) : base(message) { }
		public CompilationErrorException(string message, Exception innerException) : base(message, innerException) { }
	}
}
