namespace Sharp_LR35902_Compiler
{
	public class VariableMember : Member
	{
		public PrimitiveDataType DataType { get; }

		public VariableMember(PrimitiveDataType datatype, string name) : base(name) {
			DataType = datatype;
		}
	}
}
