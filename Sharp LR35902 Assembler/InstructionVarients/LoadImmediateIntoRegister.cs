namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadImmediateIntoRegister : InstructionVarient
	{
		public readonly Register To;
		public readonly byte Immediate;

		public LoadImmediateIntoRegister(byte immediate, Register to)
		{
			To = to;
			Immediate = immediate;
		}

		public override byte[] Compile()
		{
			var bytecode = 0x06 + ((int)To * NumRegisters);
			return new[] { (byte)bytecode, Immediate };
		}
	}
}
