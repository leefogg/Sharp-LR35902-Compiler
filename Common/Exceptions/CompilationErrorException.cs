using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public class CompilationErrorException : ErrorException
	{
		public CompilationErrorException(string message, Exception ex = null, string source = null) : base(message, ex, source) { }
	}
}
