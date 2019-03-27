namespace Sharp_LR35902_Compiler {
	public class Token {
		public TokenType Type { get; }
		public string Value { get; }

		public Token(TokenType type, string value) {
			Type = type;
			Value = value;
		}
	}
}
