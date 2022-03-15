using com.database;
using com.utilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram.dialogs
{
	internal class TelegramConfirmationDialog : TelegramDialog
	{
		private static readonly ReplyKeyboardMarkup QUESTION_KEYBOARD = TelegramUtilities.GetOneTimeKeyboard("No", "Yes");

		protected string Question { get; private set; }

		public TelegramConfirmationDialog(ChatUser chatUser, string question) : base(chatUser)
		{
			Question = question;
		}

		protected override void StartImpl()
		{
			SendTextMessageAsync($"{Question}", QUESTION_KEYBOARD);
		}

		protected override void HandleMessageImpl(int previousStep, int currentStep, Message receivedMessage)
		{
			string message = receivedMessage.Text;
			if (message == "Yes")
			{
				Complete(true);
			}
			else if(message == "No")
			{
				Complete(false);
			}
			else
			{
				SendTextMessageAsync("Your answer is interpreted as 'No'");
				Complete(false);
			}
		}

		protected override void OnStepChanging(int previousStep, int currentStep, object transitionData)
		{
			base.OnStepChanging(previousStep, currentStep, transitionData);
		}
	}
}