using System;

namespace Common.Exceptions {
	public class NotFoundException : CompilationErrorException {
		public NotFoundException(string message, Exception ex = null, string source = null) : base(message, ex, source) { }
	}
}
