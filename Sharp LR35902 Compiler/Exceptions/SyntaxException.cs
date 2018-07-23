﻿using System;

namespace Sharp_LR35902_Compiler.Exceptions
{
	public class SyntaxException :Exception
	{
		public SyntaxException() : base() { }
		public SyntaxException(string reason) : base(reason) { }
		public SyntaxException(string reason, Exception innerexception) : base(reason, innerexception) { }
	}
}
