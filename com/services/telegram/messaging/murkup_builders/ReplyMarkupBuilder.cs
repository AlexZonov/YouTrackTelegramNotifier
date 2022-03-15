using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram.messaging
{
	internal class ReplyMarkupBuilder
	{
		public bool IsEmpty { get { return Count > 0; } }
		public int Count { get; private set; }

		private List<List<KeyboardButton>> _buttons;

		public ReplyMarkupBuilder()
		{
			_buttons = new List<List<KeyboardButton>>();
		}

		public ReplyKeyboardMarkup Build(bool resizeKeyboard = true, bool oneTimeKeyboard = true)
		{
			return new ReplyKeyboardMarkup(_buttons, resizeKeyboard, oneTimeKeyboard);
		}

		public List<KeyboardButton> AddRow(params string[] buttons)
		{
			List<KeyboardButton> newRow = new List<KeyboardButton>(buttons.Length);
			for (int i = 0; i < buttons.Length; i++)
			{
				Count++;
				newRow.Add(new KeyboardButton(buttons[i]));
			}
			_buttons.Add(newRow);
			return newRow;
		}
	}
}