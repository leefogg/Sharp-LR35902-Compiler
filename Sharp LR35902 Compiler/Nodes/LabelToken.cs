using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
    public class LabelNode : Node
    {
        public string Name { get; }

        public LabelNode(string name)
        {
            Name = name;
        }

        public override IEnumerable<string> GetUsedRegisterNames()
        {
            return NoRegisters;
        }

		public override bool Equals(object obj)
		{
			if (obj is LabelNode other)
				return other.Name == Name;

			return false;
		}
	}
}
