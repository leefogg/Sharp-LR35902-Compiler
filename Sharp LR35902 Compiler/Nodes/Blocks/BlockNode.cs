using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class BlockNode : Node {
		protected List<Node> Children = new List<Node>();
		public void AddChild(Node node) => Children.Add(node);

		public void InsertAt(Node node, int index) => Children.Insert(index, node);

		public override IEnumerable<string> GetWrittenVaraibles() {
			var writtenvariables = new List<string>();
			foreach (var child in Children)
				writtenvariables.AddRange(child.GetWrittenVaraibles());

			return writtenvariables;
		}
		public override IEnumerable<string> GetReadVariables() {
			var readvariables = new List<string>();
			foreach (var child in Children)
				readvariables.AddRange(child.GetReadVariables());

			return readvariables;
		}
		public override IEnumerable<Node> GetChildren() => Children;

		public void RemoveChild(int index) => Children.RemoveAt(index);

		public override bool Matches(Node obj) {
			if (!(obj is BlockNode otherblock))
				return false;

			var otherchildren = otherblock.Children;
			if (otherchildren.Count != Children.Count)
				return false;
			for (var i = 0; i < Children.Count; i++)
				if (!Children[i].Matches(otherchildren[i]))
					return false;

			return true;

		}
	}
}
