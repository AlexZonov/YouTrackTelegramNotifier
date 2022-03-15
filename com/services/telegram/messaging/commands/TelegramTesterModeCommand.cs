using com.database;
using com.main;
using com.main.application;
using com.services.telegram.dialogs;
using com.services.telegram.messaging;

namespace com.services.telegram.commands
{
	internal class TelegramTesterModeCommand : TelegramCommand
	{
		protected Database Database { get { return App.Database; }}

		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			LinkedUser linkedUser;
			if (Database.TryGetLinkedUserByChatId(user.Id, out linkedUser))
			{
				TelegramDialog linkDialog = new TelegramTesterModeDialog(user, linkedUser);
				Telegram.UsersController.StartDialog(user.Id, linkDialog);
			}
			else
			{
				SendMessage(user.Id, TelegramMessages.YouNotLinked);
			}
		}

		protected override string GetCommandName()
		{
			return "tester_mode";
		}

		protected override string GetDescription()
		{
			return "enable or disable tester mode. You will receive a notification about the transition of any task to the \"Test\" state.";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}
	}
}