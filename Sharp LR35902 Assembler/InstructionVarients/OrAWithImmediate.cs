namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class OrAWithImmediate : InstructionVarient
	{
		public readonly byte Byte;

		public OrAWithImmediate(byte @byte)
		{
			Byte = @byte;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xF6, Byte };
		}
	}
}
