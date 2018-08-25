namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class RotateRightWithCarry : RotateInstruction
	{
		public RotateRightWithCarry(Register register) : base(0x08, register)
		{
		}
	}
}
