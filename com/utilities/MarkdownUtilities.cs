namespace com.utilities
{
	//TODO: DI from interface
	internal static class MarkdownUtilities
	{
		private static readonly string[] MARKDOWN_SYSTEM_SYMBOLS = new string[] {"_", "*"};

		public static string Escape(string inputString)
		{
			string result = inputString;
			for (int i = 0; i < MARKDOWN_SYSTEM_SYMBOLS.Length; i++)
			{
				string symbol = MARKDOWN_SYSTEM_SYMBOLS[i];
				string escapedSymbol = $"\\{symbol}";
				result = result.Replace(symbol, escapedSymbol);
			}

			return result;
		}

		public static string ToMarkdownItalic(string text)
		{
			return $"_{text}_";
		}

		public static string ToMarkdownBold(string text)
		{
			return $"*{text}*";
		}

		public static string ToMarkdownLink(string text, string link)
		{
			return $"[{text}]({link})";
		}

		public static string ToMarkdownMention(string text, int userId)
		{
			return $"[{text}](tg://user?id={userId})";
		}

		public static string ToMarkdownInlineCode(string text)
		{
			return $"`{text}`";
		}

		public static string ToMarkdownBlockCode(string text)
		{
			return $"```text {text} ```";
		}
	}
}