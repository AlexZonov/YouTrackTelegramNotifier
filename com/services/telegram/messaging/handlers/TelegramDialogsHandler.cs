using System;
using System.Collections.Generic;
using com.database;
using com.services.telegram.dialogs;
using Telegram.Bot.Types;

namespace com.services.telegram.messaging.handlers
{
	internal class TelegramDialogsHandler : TelegramAuthorizedMessagesHandler
	{
		public event Action OnSomeDialogComplete = () => { };

		private TelegramDialog _activeDialog;
		private readonly Stack<TelegramDialog> _dialoguesStack;

		public TelegramDialogsHandler(ChatUser chatUser) : base(chatUser)
		{
			_dialoguesStack = new Stack<TelegramDialog>();
		}

		public override void Dispose()
		{
			base.Dispose();
			OnSomeDialogComplete = null;
		}

		public void StartDialog(TelegramDialog dialog)
		{
			if (_activeDialog != null)
			{
				_dialoguesStack.Push(_activeDialog);
			}

			_activeDialog = dialog;

			dialog.OnCompleted += OnSomeDialogCompleted;
			dialog.Start(this);
		}

		public bool IsDialogsExist()
		{
			return _activeDialog != null;
		}

		protected override void HandleUnreadMessageImpl(Message message)
		{
			//empty
		}

		protected override void HandleSomeMessageImpl(Message message)
		{
			_activeDialog?.HandleMessage(message);
		}

		protected override void HandleSimpleMessageImpl(Message message)
		{
			//empty
		}

		protected override void HandleCommandMessageImpl(Message message, string commandName, string[] commandArgs)
		{
			//empty
		}

		private void OnSomeDialogCompleted(TelegramDialog sender)
		{
			sender.OnCompleted -= OnSomeDialogCompleted;
			sender.Dispose();

			_dialoguesStack.TryPop(out _activeDialog);

			OnSomeDialogComplete();
		}
	}
}