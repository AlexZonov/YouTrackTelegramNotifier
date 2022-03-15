using System;
using System.Threading.Tasks;
using com.database;
using com.main;
using com.main.application;
using com.services.telegram.commands;
using com.services.telegram.messaging;
using com.services.telegram.messaging.handlers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram.dialogs
{
	internal abstract class TelegramDialog : ITelegramMessagesHandler, IDisposable
	{
		public event Action<TelegramDialog> OnCompleted = (TelegramDialog sender) => { };

		public bool IsSuccess { get; private set; }
		public string Result { get; private set; }

		protected ChatUser ChatUser { get; private set; }
		protected int UserId { get { return ChatUser.Id; } }

		private int Step { get; set; }
		private int PreviousStep { get; set; }

		private TelegramDialogsHandler _dialogsHandler;
		private TelegramDialog _subDialog;
		private Action<bool, string> _subDialogCallback;

		public TelegramDialog(ChatUser chatUser)
		{
			ChatUser = chatUser;
		}

		public void Dispose()
		{
			OnCompleted = null;
		}

		public void Start(TelegramDialogsHandler dialogsHandler)
		{
			_dialogsHandler = dialogsHandler;
			StartImpl();
		}

		public void HandleMessage(Message message)
		{
			string messageLower = message.Text.ToLower();
			if (messageLower == TelegramCommands.Cancel.SpecialName ||
			    messageLower == TelegramCommands.Cancel.PresentationName || 
			    messageLower == TelegramCommands.Cancel.Name)
			{
				Complete(false);
				return;
			}

			HandleMessageImpl(PreviousStep, Step, message);
		}

		protected virtual void StartImpl()
		{
			//empty
		}

		protected virtual void HandleMessageImpl(int previousStep, int currentStep, Message receivedMessage)
		{
			//empty
		}

		protected virtual void DisposeImpl()
		{
			//empty
		}

		protected void Complete(bool isSuccess, string result = "")
		{
			IsSuccess = isSuccess;
			Result = result;
			OnCompleted(this);
		}

		protected void NextStep(object transitionData = null)
		{
			PreviousStep = Step;
			Step++;
			OnStepChanging(PreviousStep, Step, transitionData);
		}

		protected void SetStep(int newStep, object transitionData = null)
		{
			PreviousStep = Step;
			Step = newStep;
			OnStepChanging(PreviousStep, Step, transitionData);
		}

		protected virtual void OnStepChanging(int previousStep, int currentStep, object transitionData)
		{
			//empty
		}

		protected void StartSubDialog(TelegramDialog subDialog, Action<bool, string> completeCallback)
		{
			_dialogsHandler.StartDialog(subDialog);

			_subDialog = subDialog;
			_subDialogCallback = completeCallback;
			_subDialog.OnCompleted += OnSubDialogCompleted;
		}

		private void OnSubDialogCompleted(TelegramDialog sender)
		{
			_subDialogCallback?.Invoke(sender.IsSuccess, sender.Result);
			_subDialog.OnCompleted -= OnSubDialogCompleted;
			_subDialogCallback = null;
		}

		protected Task<Message> SendTextMessageAsync(string message, IReplyMarkup replyMarkup = null)
		{
			return App.Telegram.SendTextMessageAsync(UserId, message, replyMarkup);
		}

		protected Message SendTextMessage(string message, IReplyMarkup replyMarkup = null)
		{
			Task<Message> task = SendTextMessageAsync(message, replyMarkup);
			task.Wait();
			return task.Result;
		}
	}
}