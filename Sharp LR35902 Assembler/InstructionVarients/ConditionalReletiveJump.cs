namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ConditionalReletiveJump : InstructionVarient
	{
		private Condition Condition;
		private byte Direction; // TODO: Change to sbyte

		public ConditionalReletiveJump(Condition condition, byte direction)
		{
			Condition = condition;
			Direction = direction;
		}

		public override byte[] Compile()
		{
			return new[] { (byte)(0x20 + 8 * (int)Condition), Direction };
		}
	}
}
