namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ReturnWithInterrupts : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0xD9 };
		}
	}
}
