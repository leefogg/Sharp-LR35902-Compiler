using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class BlockNode : Node {
		protected List<Node> Children = new List<Node>();
		public void AddChild(Node node) { Children.Add(node); }

		public void InsertAt(Node node, int index) { Children.Insert(index, node); }

		public override Node[] GetChildren() => Children.ToArray();

		public void RemoveChild(int index) { Children.RemoveAt(index); }
	}
}
