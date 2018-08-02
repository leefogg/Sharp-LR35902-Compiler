using System.Collections.Generic;

namespace Sharp_LR35902_Compiler
{
	public class Node
	{
		private List<Node> Children = new List<Node>();

		public void AddChild(Node node) => Children.Add(node);
		public Node[] GetChildren() => Children.ToArray();
	}
}