using System;

namespace Common.Exceptions {
	public class SyntaxException : CompilationErrorException {
		public SyntaxException(string message, Exception ex = null, string source = null) : base(message, ex, source) { }
	}
}
