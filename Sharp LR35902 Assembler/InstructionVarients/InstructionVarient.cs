using System.Linq;

namespace Sharp_LR35902_Assembler.InstructionVarients
{
	abstract class InstructionVarient
	{
		internal static readonly int NumRegisters = System.Enum.GetValues(typeof(Register)).Length;


		public abstract byte[] Compile();

		public byte GetNumBytes() => (byte)Compile().Length;
	}
}
