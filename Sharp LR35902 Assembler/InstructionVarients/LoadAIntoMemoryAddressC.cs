namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadAIntoMemoryAddressC : InstructionVarient
	{
		public override byte[] Compile()
		{
			return new byte[] { 0xE2 };
		}
	}
}
