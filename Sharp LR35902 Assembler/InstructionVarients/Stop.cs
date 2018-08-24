namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class Stop : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x10 };
		}
	}
}
