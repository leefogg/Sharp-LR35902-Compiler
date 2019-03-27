using System;

namespace Common.Exceptions {
	public class SyntaxException : Exception {
		public SyntaxException() { }
		public SyntaxException(string reason) : base(reason) { }
		public SyntaxException(string reason, Exception innerexception) : base(reason, innerexception) { }
	}
}
