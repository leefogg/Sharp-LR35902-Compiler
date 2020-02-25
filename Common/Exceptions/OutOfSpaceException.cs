using System;

namespace Common.Exceptions {
	public class OutOfSpaceException : ErrorException {
		public OutOfSpaceException(string message, Exception ex = null, string source = null) : base(message, ex, source) { }
	}
}
