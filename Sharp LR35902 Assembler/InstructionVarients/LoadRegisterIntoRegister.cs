namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadRegisterIntoRegister : InstructionVarient
	{
		private readonly Register From, To;

		public LoadRegisterIntoRegister(Register to, Register from)
		{
			To = to;
			From = from;
		}

		public override byte[] Compile()
		{
			var bytecode = 0x40 + ((int)To * NumRegisters) + From;
			return new[] { (byte)bytecode };
		}
	}
}
