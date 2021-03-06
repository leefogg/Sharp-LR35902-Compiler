﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
	public abstract class ErrorException : CoreException
	{
		public ErrorException(string message) : base(message) { }
		public ErrorException(string message, Exception innerException) : base(message, innerException) { }
	}
}
