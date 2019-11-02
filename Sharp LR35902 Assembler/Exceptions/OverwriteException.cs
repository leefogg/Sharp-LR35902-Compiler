using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Sharp_LR35902_Assembler.Exceptions
{
	public class OverwriteException : Common.Exceptions.WarningException
	{
		public OverwriteException(string message) : base(message) { }
		public OverwriteException(string message, Exception innerException) : base(message, innerException) { }
	}
}
