using com.database;
using com.main;
using com.main.application;
using com.services.telegram.dialogs;
using com.services.telegram.messaging;

namespace com.services.telegram.commands
{
	internal class TelegramUnlinkCommand : TelegramCommand
	{
		protected Database Database { get { return App.Database; }}

		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			LinkedUser linkedUser;
			if (Database.TryGetLinkedUserByChatId(user.Id, out linkedUser))
			{
				TelegramDialog linkDialog = new TelegramUnlinkDialog(user, linkedUser);
				Telegram.UsersController.StartDialog(user.Id, linkDialog);
			}
			else
			{
				SendMessage(user.Id, TelegramMessages.YouNotLinked);
			}
		}

		protected override string GetCommandName()
		{
			return "unlink";
		}

		protected override string GetDescription()
		{
			return "unlink between YouTrack and Telegram profile.";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}
	}
}