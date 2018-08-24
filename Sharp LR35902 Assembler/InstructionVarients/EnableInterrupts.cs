namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class EnableInterrupts : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0xFB };
		}
	}
}
