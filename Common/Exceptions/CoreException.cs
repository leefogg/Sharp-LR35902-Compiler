using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public class CoreException : Exception
	{
		public string File;
		public CoreException(string message, Exception ex = null, string source = null) : base(message, ex) {
			File = source;
		}
	}
}
