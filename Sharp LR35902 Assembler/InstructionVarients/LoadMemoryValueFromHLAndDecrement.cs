namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadMemoryValueFromHLAndDecrement : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x3A };
		}
	}
}
