using System.Collections.Generic;

namespace Sharp_LR35902_Compiler
{
	public abstract class Node
	{
        protected readonly string[] NoRegisters = new string[0];

		protected List<Node> Children = new List<Node>();
		public void AddChild(Node node) => Children.Add(node);
		public virtual Node[] GetChildren() => Children.ToArray();
        public abstract IEnumerable<string> GetUsedRegisterNames();
	}
}