using System;

namespace Common.Exceptions {
	public class OutOfSpaceException : Exception {
		public OutOfSpaceException() { }

		public OutOfSpaceException(string message) : base(message) { }

		public OutOfSpaceException(string message, Exception innerException) : base(message, innerException) { }
	}
}
