using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ConditionalReturn : InstructionVarient
	{
		private readonly Condition Condition;

		public ConditionalReturn(Condition condition)
		{
			Condition = condition;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xC0 + ((int)Condition * 8)) };
		}
	}
}
