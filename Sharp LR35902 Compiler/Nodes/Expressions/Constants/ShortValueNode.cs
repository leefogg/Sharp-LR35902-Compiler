using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public class ShortValueNode : ConstantNode
    {
		public readonly ushort Value;

		public ShortValueNode(ushort value)
		{
			Value = value;
		}

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            return NoRegisters;
        }

		public override bool Equals(object obj)
		{
			if (obj is ShortValueNode other)
				return other.Value == Value;

			return false;
		}

		public override ushort GetValue()
			=> Value;
	}
}
