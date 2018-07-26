namespace Sharp_LR35902_Compiler
{
	public class Member
	{
		public string Name { get; }
		public string ReturnType {get;}
		public MemberType Type { get; }

		public Member(string name, MemberType type, string returntype)
		{
			Name = name;
			Type = type;
			ReturnType = returntype;
		}
	}

	public enum MemberType
	{
		Method,
		Variable,
	}
}
