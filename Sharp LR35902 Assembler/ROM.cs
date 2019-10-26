namespace Sharp_LR35902_Assembler {
	public class ROM {
		private readonly byte[] Bytecode = new byte[1024 * 16];

		public ROM() { }
		public ROM(byte padding)
		{
			for (var i = 0; i < Bytecode.Length; i++)
				Bytecode[i] = padding;
		}

		public byte this[int index] {
			get => Bytecode[index];
			set => Bytecode[index] = value;
		}


		public static implicit operator byte[](ROM rom) => rom.Bytecode;
	}
}
