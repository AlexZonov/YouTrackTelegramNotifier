using System;
using com.database;
using com.main;
using com.main.application;
using com.services.telegram.commands;
using com.services.youtrack;
using com.services.youtrack.rest;
using com.utilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram.dialogs
{
	internal class TelegramLinkDialog : TelegramDialog
	{
		private const string AGAIN_BUTTON = "Again";
		private static readonly ReplyKeyboardMarkup INVALID_LOGIN_KEYBOARD = TelegramUtilities.GetOneTimeKeyboard(TelegramCommands.Cancel.PresentationName, AGAIN_BUTTON);

		protected YouTrackService YouTrack { get { return App.YouTrack; } }
		protected Database Database { get { return App.Database; }}

		public TelegramLinkDialog(ChatUser chatUser) : base(chatUser)
		{
			//empty
		}

		protected override void StartImpl()
		{
			LinkedUser linkedUser;
			if (Database.TryGetLinkedUserByChatId(UserId, out linkedUser))
			{
				TelegramConfirmationDialog confirmDialog = new TelegramConfirmationDialog(ChatUser, $"You already linked as '{linkedUser.YouTrackLogin}', Want to replace?");
				StartSubDialog(confirmDialog, OnReplaceConfirmDialogComplete);
			}
			else
			{
				SetStep(LinkDialogStep.AwaitingYoutrackLogin);
			}
		}

		protected override void HandleMessageImpl(int previousStep, int currentStep, Message receivedMessage)
		{
			LinkDialogStep previousLinkStep = (LinkDialogStep) previousStep;
			LinkDialogStep currentLinkStep = (LinkDialogStep) currentStep;

			string receivedMessageText = receivedMessage.Text;

			if (currentLinkStep == LinkDialogStep.AwaitingYoutrackLogin)
			{
				LinkedUser linkedUser;
				bool isLinkedUserExist = Database.TryGetLinkedUserByChatId(UserId, out linkedUser);

				YouTrackRESTUser youTrackUser;
				if (YouTrack.REST.TryGetUser(receivedMessageText, out youTrackUser))
				{
					if (!isLinkedUserExist)
					{
						Database.AddLinkedUser(receivedMessageText, UserId);
						SendTextMessageAsync($"You have successfully linked! For unlink use /unlink command.\r\nLogin: '{receivedMessageText}'\r\nUser ID: '{UserId}'.");
						Complete(true);
					}
					else
					{
						string currentLogin = linkedUser.YouTrackLogin;
						linkedUser.ReplaceLogin(receivedMessageText);
						SendTextMessageAsync($"Login replaced from '{currentLogin}' to '{receivedMessageText}'");
						Complete(true);
					}
				}
				else
				{
					SetStep(LinkDialogStep.AwaitingActionOnFail, receivedMessageText);
				}
			}
			else if (currentLinkStep == LinkDialogStep.AwaitingActionOnFail)
			{
				if (receivedMessageText == AGAIN_BUTTON)
				{
					SetStep(LinkDialogStep.AwaitingYoutrackLogin);
				}
				else if (receivedMessageText != TelegramCommands.Cancel.PresentationName)
				{
					SendTextMessageAsync($"Your answer is interpreted as '{TelegramCommands.Cancel.PresentationName}'");
					Complete(false);
				}
			}
		}

		protected override void OnStepChanging(int previousStep, int currentStep, object transitionData)
		{
			base.OnStepChanging(previousStep, currentStep, transitionData);

			LinkDialogStep previousLinkStep = (LinkDialogStep) previousStep;
			LinkDialogStep currentLinkStep = (LinkDialogStep) currentStep;

			switch (currentLinkStep)
			{
				case LinkDialogStep.AwaitingYoutrackLogin:
				{
					SendTextMessageAsync("Please enter the your login YouTrack");
					break;
				}
				case LinkDialogStep.AwaitingActionOnFail:
				{
					string messageText = $"Login: '{(string)transitionData}' not found! Try again or cancel the operation.";
					SendTextMessageAsync(messageText, INVALID_LOGIN_KEYBOARD);
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		private void OnReplaceConfirmDialogComplete(bool isSuccess, string result)
		{
			if (isSuccess)
			{
				SetStep(LinkDialogStep.AwaitingYoutrackLogin);
			}
			else
			{
				Complete(false);
			}
		}

		private void SetStep(LinkDialogStep newStep, object transitionData = null)
		{
			base.SetStep((int) newStep, transitionData);
		}
	}

	public enum LinkDialogStep
	{
		AwaitingYoutrackLogin,
		AwaitingActionOnFail
	}
}