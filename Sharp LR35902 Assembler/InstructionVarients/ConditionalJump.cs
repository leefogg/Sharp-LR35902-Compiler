using Common.Extensions;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class ConditionalJump : InstructionVarient
	{
		private readonly Condition Condition;
		private readonly byte LowerAddress, HigherAddress;

		public ConditionalJump(Condition condition, ushort address)
		{
			Condition = condition;
			var addressbytes = address.ToByteArray();
			LowerAddress = addressbytes[0];
			HigherAddress = addressbytes[1];
		}

		public override byte[] Compile()
		{
			return new byte[] { (byte)(0xC2 + 8 * (int)Condition), LowerAddress, HigherAddress };
		}
	}
}
