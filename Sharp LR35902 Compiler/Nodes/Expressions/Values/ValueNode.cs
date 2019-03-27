namespace Sharp_LR35902_Compiler.Nodes {
	public abstract class ValueNode : ExpressionNode {
		public override Node[] GetChildren() => NoChildren;
	}
}
