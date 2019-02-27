using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class BlockNode : Node
	{
		protected List<Node> Children = new List<Node>();
		public void AddChild(Node node) => Children.Add(node);
		public override Node[] GetChildren() => Children.ToArray();
	}
}
