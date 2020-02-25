using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public abstract class ErrorException : CoreException
	{
		public ErrorException(string message, Exception ex = null, string source = null) : base(message, ex, source) { }
	}
}
