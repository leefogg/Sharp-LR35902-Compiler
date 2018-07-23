using System;

namespace Sharp_LR35902_Compiler.Exceptions
{
	public class NotFoundException : Exception
	{
		public NotFoundException() : base() { }
		public NotFoundException(string reason) : base(reason) { }
		public NotFoundException(string reason, Exception innerexception) : base(reason, innerexception) { }
	}
}
