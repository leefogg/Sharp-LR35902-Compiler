using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class BlockNode : Node {
		protected List<Node> Children = new List<Node>();
		public void AddChild(Node node) => Children.Add(node);

		public void InsertAt(Node node, int index) => Children.Insert(index, node);

		public override Node[] GetChildren() => Children.ToArray();

		public void RemoveChild(int index) => Children.RemoveAt(index);

		public override IEnumerable<string> GetUsedRegisterNames()
		{
			foreach (var child in Children)
				foreach (var variablename in child.GetUsedRegisterNames())
					yield return variablename;
		}

		public override bool Equals(object obj) {
			if (obj is BlockNode otherblock) {
				var otherchildren = otherblock.Children;
				if (otherchildren.Count != Children.Count)
					return false;
				for (var i = 0; i < Children.Count; i++)
					if (!Children[i].Equals(otherchildren[i]))
						return false;

				return true;
			}

			return false;
		}
	}
}
