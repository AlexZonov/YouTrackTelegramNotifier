using System;
using System.Text.RegularExpressions;

namespace com.utilities
{
	internal static class StringUtilities
	{
		private static readonly Regex _varPresentationRegex = new Regex(@"(?<part>[A-Z][a-z]{0,})", RegexOptions.Multiline);
		
		public static string FirstCharToUpper(this string input)
		{
			switch (input)
			{
				case null: throw new ArgumentNullException(nameof(input));
				case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
				default: return input[0].ToString().ToUpper() + input.Substring(1);
			}
		}

		public static string GetVariablePresentation(string variable, bool isSentenceMode)
		{
			int i = 0;
			return _varPresentationRegex.Replace(variable, match =>
			{
				string value = match.Groups["part"].Value;
				if (isSentenceMode && i > 0)
				{
					value = value.ToLower();
				}

				i++;
				return $"{value} ";
			}).TrimEnd(' ');
		}
	}
}