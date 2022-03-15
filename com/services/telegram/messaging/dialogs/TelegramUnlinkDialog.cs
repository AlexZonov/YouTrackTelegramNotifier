using com.database;
using com.main;
using com.main.application;
using com.services.telegram.commands;

namespace com.services.telegram.dialogs
{
	internal class TelegramUnlinkDialog : TelegramConfirmationDialog
	{
		protected Database Database { get { return App.Database; }}

		private LinkedUser _linkedUser;

		public TelegramUnlinkDialog(ChatUser chatUser, LinkedUser linkedUser) : base(chatUser, $"Are you sure you want to unlink the \"{linkedUser.YouTrackLogin}\" account?")
		{
			_linkedUser = linkedUser;
			OnCompleted += OnCompletedHandler;
		}

		private void OnCompletedHandler(TelegramDialog sender)
		{
			if (IsSuccess)
			{
				Database.RemoveLinkedUser(UserId);
				SendTextMessageAsync($"You have successfully removed the link to the '{_linkedUser.YouTrackLogin}' account. For link use {TelegramCommands.Link.SpecialName} command.");
			}
		}
	}
}