using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class Node {
		protected readonly Node[] NoChildren = new Node[0];
		protected readonly string[] NoVariables = new string[0];

		public IEnumerable<string> GetUsedVariables() => GetReadVariables().Concat(GetWrittenVaraibles());
		public abstract IEnumerable<string> GetWrittenVaraibles();
		public abstract IEnumerable<string> GetReadVariables();

		public abstract IEnumerable<Node> GetChildren();

		public abstract bool Matches(Node other);
	}
}
