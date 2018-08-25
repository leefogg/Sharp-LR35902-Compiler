namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ConditionalReturn : InstructionVarient
	{
		public readonly Condition Condition;

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
