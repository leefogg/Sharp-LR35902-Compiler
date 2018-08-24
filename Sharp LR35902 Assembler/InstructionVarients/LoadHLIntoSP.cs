namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadHLIntoSP : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0xF9 };
		}
	}
}
