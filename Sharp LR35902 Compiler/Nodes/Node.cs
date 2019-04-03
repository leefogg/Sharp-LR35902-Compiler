using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class Node {
		protected readonly Node[] NoChildren = new Node[0];
		protected readonly string[] NoRegisters = new string[0];

		public abstract IEnumerable<string> GetUsedRegisterNames();

		public abstract IEnumerable<Node> GetChildren();
	}
}
