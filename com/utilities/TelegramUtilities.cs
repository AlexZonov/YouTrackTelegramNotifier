using com.services.youtrack.custom.issue;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.utilities
{
	internal static class TelegramUtilities
	{
		public static ReplyKeyboardMarkup GetOneTimeKeyboard(params string[] buttonsNames)
		{
			KeyboardButton[] buttons = new KeyboardButton[buttonsNames.Length];
			for (int i = 0; i < buttonsNames.Length; i++)
			{
				buttons[i] = new KeyboardButton(buttonsNames[i]);
			}

			return GetOneTimeKeyboard(buttons);
		}

		public static ReplyKeyboardMarkup GetOneTimeKeyboard(params KeyboardButton[] buttons)
		{
			return new ReplyKeyboardMarkup(buttons, true, true);
		}

		public static string GetPriorityEmoji(YouTrackIssuePriority priority)
		{
			if(priority == YouTrackIssuePriority.Low)
			{
				return Symbols.WhiteCube;
			}
			else if(priority == YouTrackIssuePriority.Normal)
			{
				return Symbols.GreenCube;
			}
			else if(priority == YouTrackIssuePriority.High)
			{
				return Symbols.OrangeCube;
			}
			else if(priority == YouTrackIssuePriority.Critical)
			{
				return Symbols.RedCube;
			}
			else
			{
				return Symbols.GrayQuestion;
			}
		}
	}
}