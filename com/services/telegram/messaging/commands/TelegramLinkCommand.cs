using com.database;
using com.services.telegram.dialogs;

namespace com.services.telegram.commands
{
	internal class TelegramLinkCommand : TelegramCommand
	{
		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			TelegramDialog linkDialog = new TelegramLinkDialog(user);
			Telegram.UsersController.StartDialog(user.Id, linkDialog);
		}

		protected override string GetCommandName()
		{
			return "link";
		}

		protected override string GetDescription()
		{
			return "set link between YouTrack and Telegram profile.";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}
	}
}