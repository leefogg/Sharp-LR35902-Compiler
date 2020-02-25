using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public abstract class WarningException : CoreException
	{
		public WarningException(string message, Exception ex = null, string source = null) : base(message, ex, source) { }
	}
}
