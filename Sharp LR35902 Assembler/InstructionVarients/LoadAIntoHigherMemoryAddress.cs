namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadAIntoHigherMemoryAddress : InstructionVarient
	{
		private byte Address;

		public LoadAIntoHigherMemoryAddress(byte address)
		{
			Address = address;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xE0, Address };
		}
	}
}
