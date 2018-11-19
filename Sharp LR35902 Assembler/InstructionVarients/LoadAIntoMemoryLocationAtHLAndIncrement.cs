namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadAIntoMemoryLocationAtHLAndIncrement : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x22 };
		}
	}
}
