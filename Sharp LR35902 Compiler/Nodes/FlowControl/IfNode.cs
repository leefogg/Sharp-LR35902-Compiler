using System.Collections.Generic;
using System.Linq;

namespace Sharp_LR35902_Compiler.Nodes {
	public class IfNode : Node {
		public ExpressionNode Condition { get; set; }
		public BlockNode IfTrue { get; set; }
		public BlockNode IfFalse { get; set; }

		public IfNode(ExpressionNode condition, BlockNode ifTrue) : this(condition, ifTrue, null) { }
		public IfNode(ExpressionNode condition, BlockNode iftrue, BlockNode iffalse) {
			Condition = condition;
			IfTrue = iftrue ?? new BlockNode();
			IfFalse = iffalse ?? new BlockNode();
		}

		public override IEnumerable<string> GetUsedRegisterNames() {
			foreach (var variablename in Condition.GetUsedRegisterNames())
				yield return variablename;
			foreach (var variablename in IfTrue.GetUsedRegisterNames())
				yield return variablename;
			foreach (var variablename in IfFalse.GetUsedRegisterNames())
				yield return variablename;
		}

		public override bool Matches(Node obj) {
			if (obj is IfNode other)
				return other.Condition.Matches(Condition) && other.IfTrue.Matches(IfTrue) && other.IfFalse.Matches(IfFalse);

			return false;
		}

		public override IEnumerable<Node> GetChildren() => Condition.GetChildren().Concat(IfTrue.GetChildren()).Concat(IfFalse.GetChildren());
	}
}
