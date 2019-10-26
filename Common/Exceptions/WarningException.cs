using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public abstract class WarningException : Exception
	{
		public WarningException(string message) : base(message) { }
		public WarningException(string message, Exception innerException) : base(message, innerException) { }
	}
}
