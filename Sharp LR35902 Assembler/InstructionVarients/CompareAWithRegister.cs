namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class CompareAWithRegister : InstructionVarient
	{
		public readonly Register Register;

		public CompareAWithRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xB8 + (int)Register) };
		}
	}
}
