using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public class OverwriteException : WarningException
	{
		public OverwriteException(string message) : base(message) { }
		public OverwriteException(string message, Exception innerException) : base(message, innerException) { }
	}
}
