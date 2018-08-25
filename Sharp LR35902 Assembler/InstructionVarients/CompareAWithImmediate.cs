namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class CompareAWithImmediate : InstructionVarient
	{
		public readonly byte Byte;

		public CompareAWithImmediate(byte @byte)
		{
			Byte = @byte;
		}

		public override byte[] Compile()
		{
			return new byte[] { 0xFE, Byte };
		}
	}
}
