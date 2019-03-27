using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public class ASTNode : BlockNode {
		public override IEnumerable<string> GetUsedRegisterNames() {
			foreach (var child in Children)
				foreach (var variablename in child.GetUsedRegisterNames())
					yield return variablename;
		}
	}
}
