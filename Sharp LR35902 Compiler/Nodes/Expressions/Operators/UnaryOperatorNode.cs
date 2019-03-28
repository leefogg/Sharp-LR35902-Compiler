namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class UnaryOperatorNode : OperatorNode
	{
		public ExpressionNode Expression { get; set; }

		public override Node[] GetChildren() => NoChildren;
	}
}
