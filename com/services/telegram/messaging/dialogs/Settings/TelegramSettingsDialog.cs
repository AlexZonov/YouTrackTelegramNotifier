using com.database;
using com.main;
using com.main.application;
using com.services.telegram.commands;
using com.services.telegram.messaging;
using com.services.youtrack;
using com.utilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram.dialogs
{
	internal class TelegramSettingsDialog : TelegramDialog
	{
		private const string ANOTHER_BUTTON = "Another";
		private const string AGAIN_BUTTON = "Again";
		private const string ENABLE_BUTTON = "Enable";
		private const string DISABLE_BUTTON = "Disable";
		private const string BACK_BUTTON = "Back to choose action";

		private static readonly ReplyKeyboardMarkup FAIL_KEYBOARD = TelegramUtilities.GetOneTimeKeyboard(TelegramCommands.Cancel.PresentationName, BACK_BUTTON, AGAIN_BUTTON);
		private static readonly ReplyKeyboardMarkup SUCCESS_KEYBOARD = TelegramUtilities.GetOneTimeKeyboard(TelegramCommands.Cancel.PresentationName, ANOTHER_BUTTON);
		private static readonly ReplyKeyboardMarkup CHOSE_ACTION_KEYBOARD = TelegramUtilities.GetOneTimeKeyboard(TelegramCommands.Cancel.PresentationName, DISABLE_BUTTON, ENABLE_BUTTON);

		protected YouTrackService YouTrack { get { return App.YouTrack; } }
		protected Database Database { get { return App.Database; }}

		private LinkedUser _linkedUser;
		private bool _isDisableMode;
		private ChooseNotificationRequest _chooseRequest;

		public TelegramSettingsDialog(ChatUser chatUser) : base(chatUser)
		{
			//empty
		}

		protected override void StartImpl()
		{
			if (Database.TryGetLinkedUserByChatId(ChatUser.Id, out _linkedUser))
			{
				SetStep(SettingsDialogStep.AwaitingChoseAction);
			}
			else
			{
				SendTextMessageAsync(TelegramMessages.YouNotLinked);
				Complete(false);
			}
		}

		protected override void HandleMessageImpl(int previousStep, int currentStep, Message receivedMessage)
		{
			SettingsDialogStep previousLinkStep = (SettingsDialogStep) previousStep;
			SettingsDialogStep currentLinkStep = (SettingsDialogStep) currentStep;

			string receivedText = receivedMessage.Text;

			if (currentLinkStep == SettingsDialogStep.AwaitingChoseAction)
			{
				if (receivedText == ENABLE_BUTTON)
				{
					_isDisableMode = false;
					SetStep(SettingsDialogStep.AwaitingType);
				}
				else if(receivedText == DISABLE_BUTTON)
				{
					_isDisableMode = true;
					SetStep(SettingsDialogStep.AwaitingType);
				}
				else if(receivedText != TelegramCommands.Cancel.PresentationName)
				{
					SendTextMessageAsync($"Your answer is interpreted as '{TelegramCommands.Cancel.PresentationName}'");
					Complete(false);
				}
			}
			else if (currentLinkStep == SettingsDialogStep.AwaitingType)
			{
				if (receivedText == BACK_BUTTON)
				{
					SetStep(SettingsDialogStep.AwaitingChoseAction);
				}
				else
				{
					ChoseNotificationResponseHandler notificationResponseHandler = new ChoseNotificationResponseHandler(receivedText, _chooseRequest);
					bool isSuccessAction = notificationResponseHandler.Record != null;
					if (isSuccessAction)
					{
						_chooseRequest = null;
						_linkedUser.SetNotificationEnable(notificationResponseHandler.Record.Type, !_isDisableMode);

						if (_isDisableMode)
						{
							//
						}
						else
						{
							//
						}
						SendTextMessageAsync($"You successfully {GetActionName(_isDisableMode)} the notification: \"{notificationResponseHandler.Record.Type}\". Select an action.", SUCCESS_KEYBOARD);
						SetStep(SettingsDialogStep.AwaitingSuccessAction);
					}
					else
					{
						SendTextMessageAsync($"Invalid response: \"{receivedText}\". Select an action.", FAIL_KEYBOARD);
						SetStep(SettingsDialogStep.AwaitingFailAction);
					}
				}
			}
			else if (currentLinkStep == SettingsDialogStep.AwaitingSuccessAction)
			{
				if (receivedText == ANOTHER_BUTTON)
				{
					SetStep(SettingsDialogStep.AwaitingType);
				}
				else if(receivedText != TelegramCommands.Cancel.PresentationName)
				{
					SendTextMessageAsync($"Your answer is interpreted as '{TelegramCommands.Cancel.PresentationName}'");
					Complete(false);
				}
			}
			else if (currentLinkStep == SettingsDialogStep.AwaitingFailAction)
			{
				if (receivedText == AGAIN_BUTTON)
				{
					SetStep(SettingsDialogStep.AwaitingType);
				}
				else if (receivedText == BACK_BUTTON)
				{
					SetStep(SettingsDialogStep.AwaitingChoseAction);
				}
				else if(receivedText != TelegramCommands.Cancel.PresentationName)
				{
					SendTextMessageAsync($"Your answer is interpreted as '{TelegramCommands.Cancel.PresentationName}'");
					Complete(false);
				}
			}
		}

		protected override void OnStepChanging(int previousStep, int currentStep, object transitionData)
		{
			base.OnStepChanging(previousStep, currentStep, transitionData);

			SettingsDialogStep previousLinkStep = (SettingsDialogStep) previousStep;
			SettingsDialogStep currentLinkStep = (SettingsDialogStep) currentStep;

			switch (currentLinkStep)
			{
				case SettingsDialogStep.AwaitingChoseAction:
				{
					SendTextMessageAsync($"Choose action: ", CHOSE_ACTION_KEYBOARD);
					break;
				}
				case SettingsDialogStep.AwaitingType:
				{
					_chooseRequest = new ChooseNotificationRequest(_linkedUser, _isDisableMode);
					if (_chooseRequest.IsEmpty)
					{
						SendTextMessage($"No notification for \"{GetActionName(_isDisableMode)}\" action!");
						SetStep(SettingsDialogStep.AwaitingChoseAction);
					}
					else
					{
						SendTextMessageAsync("Choose notification type:", GetActionKeyboard(_chooseRequest));
					}
					break;
				}
			}
		}

		private string GetActionName(bool isDisableMode)
		{
			return isDisableMode ? "disable" : "enable";
		}

		private void SetStep(SettingsDialogStep newStep, object transitionData = null)
		{
			base.SetStep((int) newStep, transitionData);
		}

		private IReplyMarkup GetActionKeyboard(ChooseNotificationRequest request)
		{
			ReplyMarkupBuilder replyMarkupBuilder = new ReplyMarkupBuilder();

			replyMarkupBuilder.AddRow(TelegramCommands.Cancel.PresentationName, BACK_BUTTON);
			foreach (var recordPair in request.Records)
			{
				ChooseNotificationRecord record = recordPair.Value;
				replyMarkupBuilder.AddRow(record.Presentation);
			}

			return replyMarkupBuilder.Build();
		}

		private void OneTimeMessage(string message, IReplyMarkup markup = null)
		{
			Message sendedMessage = SendTextMessage(message, markup);
			App.Telegram.BotClient.DeleteMessageAsync(sendedMessage.Chat.Id, sendedMessage.MessageId).Wait();
		}
	}
}