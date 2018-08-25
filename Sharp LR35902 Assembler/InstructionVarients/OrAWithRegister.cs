namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class OrAWithRegister : InstructionVarient
	{
		public readonly Register Register;

		public OrAWithRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xB0 + (int)Register) };
		}
	}
}
