namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadMemoryValueFromHLAndIncrement : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x2A };
		}
	}
}
