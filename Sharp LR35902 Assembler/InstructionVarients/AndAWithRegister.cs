namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class AndAWithRegister : InstructionVarient
	{
		public readonly Register Register;

		public AndAWithRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xA0 + (int)Register) };
		}
	}
}
