namespace Common.Extensions {
	public static class CharExtensions {
		public static bool IsLetter(this char self) => self >= 'a' && self <= 'z' || self >= 'A' && self <= 'Z';

		public static bool IsNumber(this char self) => self >= '0' && self <= '9';
	}
}
