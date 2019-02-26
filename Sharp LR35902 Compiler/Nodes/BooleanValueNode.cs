namespace Sharp_LR35902_Compiler.Nodes
{
	public class BooleanValueNode : ShortValueNode
	{
		public BooleanValueNode(bool value) : base(value ? (ushort)1 : (ushort)0)
		{
		}
	}
}
