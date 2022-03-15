using com.database;
using com.main;
using com.main.application;
using com.services.telegram.dialogs;

namespace com.services.telegram.commands
{
	internal class TelegramSettingsCommand : TelegramCommand
	{
		protected Database Database { get { return App.Database; }}

		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			TelegramDialog muteDialog = new TelegramSettingsDialog(user);
			Telegram.UsersController.StartDialog(user.Id, muteDialog);
		}

		protected override string GetCommandName()
		{
			return "settings";
		}

		protected override string GetDescription()
		{
			return "settings for notification(enable or disable).";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}
	}
}