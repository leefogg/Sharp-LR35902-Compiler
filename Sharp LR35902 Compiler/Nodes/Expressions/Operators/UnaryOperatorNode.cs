using System.Collections.Generic;

namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class UnaryOperatorNode : OperatorNode {
		public ExpressionNode Expression { get; set; }

		public override IEnumerable<Node> GetChildren() => NoChildren;

		public override IEnumerable<string> GetReadVariables() => Expression.GetReadVariables();

		public override string ToString() => GetSymbol() + Expression.ToString();
		protected abstract char GetSymbol();
	}
}
