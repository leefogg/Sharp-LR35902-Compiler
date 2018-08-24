namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadAIntoMemoryAddressAtDE : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0x12 };
		}
	}
}
