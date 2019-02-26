namespace Sharp_LR35902_Compiler.Nodes
{
	public abstract class OperatorNode : ExpressionNode
	{
		public ExpressionNode Left { get; set; }
		public ExpressionNode Right { get; set; }
	}
}