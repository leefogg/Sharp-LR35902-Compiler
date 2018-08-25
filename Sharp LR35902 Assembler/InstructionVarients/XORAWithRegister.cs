namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class XORAWithRegister : InstructionVarient
	{
		public readonly Register Register;

		public XORAWithRegister(Register register)
		{
			Register = register;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0xA8 + (int)Register) };
		}
	}
}
