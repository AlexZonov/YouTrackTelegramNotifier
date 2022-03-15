using System.Web;

namespace com.utilities
{
	//TODO: DI from interface
	internal static class HtmlUtilities
	{
		public static string Escape(string inputString)
		{
			return HttpUtility.HtmlEncode(inputString);
		}

		public static string ToHtmlItalic(string text)
		{
			return $"<i>{text}</i>";
		}

		public static string ToHtmlBold(string text)
		{
			return $"<b>{text}</b>";
		}

		public static string ToHtmlLink(string text, string link)
		{
			return $"<a href=\"{link}\">{text}</a>";
		}

		public static string ToHtmlMention(string text, int userId)
		{
			return $"<a href=\"tg://user?id={userId}\">{text}</a>";
		}

		public static string ToHtmlInlineCode(string text)
		{
			return $"<code>{text}</code>";
		}

		public static string ToHtmlBlockCode(string text)
		{
			return $"<pre>{text}</pre>";
		}
	}
}