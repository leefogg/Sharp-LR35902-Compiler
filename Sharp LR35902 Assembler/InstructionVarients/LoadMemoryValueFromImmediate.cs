using Common.Extensions;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	class LoadMemoryValueFromImmediate : InstructionVarient
	{
		public readonly ushort Location;

		// Only really valid for BC and DE
		public LoadMemoryValueFromImmediate(ushort location)
		{
			Location = location;
		}

		public override byte[] Compile()
		{
			var bytes = Location.ToByteArray();
			return new byte[] { 0xFA, bytes[0], bytes[1] };
		}
	}
}
