namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class RotateLeft : RotateInstruction
	{
		public RotateLeft(Register register) : base(0x10, register)
		{
		}
	}
}
