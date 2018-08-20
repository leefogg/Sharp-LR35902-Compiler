using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp_LR35902_Assembler
{
    public class ROM {
		private byte[] bytecode = new byte[1024 * 16];

		public byte this[int index] {
			get => bytecode[index];
			set {
				// TODO: Check and grow rom to next MBC size if out of bounds
				bytecode[index] = value;
			}
		}

		public static implicit operator byte[](ROM rom) => rom.bytecode;
	}
}
