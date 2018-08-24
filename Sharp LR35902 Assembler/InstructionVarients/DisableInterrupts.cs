namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class DisableInterrupts : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0xF3 };
		}
	}
}
