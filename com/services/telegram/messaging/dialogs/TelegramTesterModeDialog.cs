using com.database;
using com.main;
using com.main.application;
using com.services.telegram.commands;
using com.services.youtrack;
using com.utilities;
using Telegram.Bot.Types;

namespace com.services.telegram.dialogs
{
	internal class TelegramTesterModeDialog : TelegramDialog
	{
		private const string DISABLE_BUTTON = "Disable";
		private const string ENABLE_BUTTON = "Enable";

		protected YouTrackService YouTrack { get { return App.YouTrack; } }
		protected Database Database { get { return App.Database; }}

		private LinkedUser _linkedUser;

		public TelegramTesterModeDialog(ChatUser chatUser, LinkedUser linkedUser) : base(chatUser)
		{
			_linkedUser = linkedUser;
		}

		protected override void StartImpl()
		{
			SetStep(TesterModeDialogStep.AwaitingChooseAction);
		}

		protected override void HandleMessageImpl(int previousStep, int currentStep, Message receivedMessage)
		{
			TesterModeDialogStep previousLinkStep = (TesterModeDialogStep) previousStep;
			TesterModeDialogStep currentLinkStep = (TesterModeDialogStep) currentStep;

			string receivedMessageText = receivedMessage.Text;

			if (currentLinkStep == TesterModeDialogStep.AwaitingChooseAction)
			{
				if (receivedMessageText == ENABLE_BUTTON)
				{
					SendTextMessageAsync($"You have successfully enabled tester mode!");
					_linkedUser.SetTester(true);
					Complete(true);
				}
				else if (receivedMessageText == DISABLE_BUTTON)
				{
					SendTextMessageAsync($"You have successfully disabled tester mode!");
					_linkedUser.SetTester(false);
					Complete(true);
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

			TesterModeDialogStep previousLinkStep = (TesterModeDialogStep) previousStep;
			TesterModeDialogStep currentLinkStep = (TesterModeDialogStep) currentStep;

			switch (currentLinkStep)
			{
				case TesterModeDialogStep.AwaitingChooseAction:
				{
					SendTextMessageAsync("Select an action:", TelegramUtilities.GetOneTimeKeyboard(TelegramCommands.Cancel.PresentationName, _linkedUser.IsTester ? DISABLE_BUTTON : ENABLE_BUTTON));
					break;
				}
			}
		}

		private void SetStep(TesterModeDialogStep newStep, object transitionData = null)
		{
			base.SetStep((int) newStep, transitionData);
		}
	}

	public enum TesterModeDialogStep
	{
		AwaitingChooseAction,
	}
}