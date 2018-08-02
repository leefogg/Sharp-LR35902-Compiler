namespace Sharp_LR35902_Compiler
{
	public class VariableMember : Member
	{
		public string DataType { get; }

		public VariableMember(string datatype, string name) : base(name) {
			DataType = datatype;
		}
	}
}
