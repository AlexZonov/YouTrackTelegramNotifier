using com.main;
using com.main.application;
using Telegram.Bot.Types.Enums;

namespace com.utilities
{
	internal static class ParseTelegramMessageUtilities
	{
		public static string Escape(string inputString)
		{
			return Escape(inputString, App.Config.Telegram.ParseMode);
		}

		public static string ToItalic(this string text)
		{
			return ToItalic(text, App.Config.Telegram.ParseMode);
		}

		public static string ToBold(this string text)
		{
			return ToBold(text, App.Config.Telegram.ParseMode);
		}

		public static string ToLink(this string text, string link)
		{
			return ToLink(text, link, App.Config.Telegram.ParseMode);
		}

		public static string ToMention(this string text, int userId)
		{
			return ToMention(text, userId, App.Config.Telegram.ParseMode);
		}

		public static string ToInlineCode(this string text)
		{
			return ToInlineCode(text, App.Config.Telegram.ParseMode);
		}

		public static string ToBlockCode(this string text)
		{
			return ToBlockCode(text, App.Config.Telegram.ParseMode);
		}

		public static string Escape(this string text, ParseMode mode)
		{
			if (mode == ParseMode.Html)
			{
				return HtmlUtilities.Escape(text);
			}
			else if (mode == ParseMode.Markdown)
			{
				return MarkdownUtilities.Escape(text);
			}
			else
			{
				return text;
			}
		}

		public static string ToItalic(this string text, ParseMode mode)
		{
			if (mode == ParseMode.Html)
			{
				return HtmlUtilities.ToHtmlItalic(text);
			}
			else if (mode == ParseMode.Markdown)
			{
				return MarkdownUtilities.ToMarkdownItalic(text);
			}
			else
			{
				return text;
			}
		}

		public static string ToBold(this string text, ParseMode mode)
		{
			if (mode == ParseMode.Html)
			{
				return HtmlUtilities.ToHtmlBold(text);
			}
			else if (mode == ParseMode.Markdown)
			{
				return MarkdownUtilities.ToMarkdownBold(text);
			}
			else
			{
				return text;
			}
		}

		public static string ToLink(this string text, string link, ParseMode mode)
		{
			if (mode == ParseMode.Html)
			{
				return HtmlUtilities.ToHtmlLink(text, link);
			}
			else if (mode == ParseMode.Markdown)
			{
				return MarkdownUtilities.ToMarkdownLink(text, link);
			}
			else
			{
				return text;
			}
		}

		public static string ToMention(this string text, int userId, ParseMode mode)
		{
			if (mode == ParseMode.Html)
			{
				return HtmlUtilities.ToHtmlMention(text, userId);
			}
			else if (mode == ParseMode.Markdown)
			{
				return MarkdownUtilities.ToMarkdownMention(text, userId);
			}
			else
			{
				return text;
			}
		}

		public static string ToInlineCode(this string text, ParseMode mode)
		{
			if (mode == ParseMode.Html)
			{
				return HtmlUtilities.ToHtmlInlineCode(text);
			}
			else if (mode == ParseMode.Markdown)
			{
				return MarkdownUtilities.ToMarkdownInlineCode(text);
			}
			else
			{
				return text;
			}
		}

		public static string ToBlockCode(this string text, ParseMode mode)
		{
			if (mode == ParseMode.Html)
			{
				return HtmlUtilities.ToHtmlBlockCode(text);
			}
			else if (mode == ParseMode.Markdown)
			{
				return MarkdownUtilities.ToMarkdownBlockCode(text);
			}
			else
			{
				return text;
			}
		}
	}
}