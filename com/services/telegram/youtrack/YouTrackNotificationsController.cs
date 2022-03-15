using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.database;
using com.services.youtrack;
using com.services.youtrack.custom;
using com.services.youtrack.custom.events;
using com.services.youtrack.custom.issue;
using com.utilities;
using DryIoc;
using Telegram.Bot.Types.Enums;

namespace com.services.telegram.youtrack
{
	internal class YouTrackNotificationsController
	{
		private TelegramService _telegram;
		private Database _database;
		private YouTrackIssuesEventsBuffer _issuesEventsBuffer;

		public YouTrackNotificationsController(IContainer container)
		{
			_telegram = container.Resolve<TelegramService>();
			_database = container.Resolve<Database>();

			_issuesEventsBuffer = container.Resolve<YouTrackIssuesEventsBuffer>();

			_issuesEventsBuffer.OnIssueAssigneeChangeEventReceived += OnIssueEventsAssigneeChanged;
			_issuesEventsBuffer.OnIssueSummaryChangeEventReceived += OnIssueEventsSummaryChanged;
			_issuesEventsBuffer.OnIssueDescriptionChangeEventReceived += OnIssueEventsDescriptionChanged;
			_issuesEventsBuffer.OnIssueSimpleFieldChangeEventReceived += OnIssueEventsSimpleFieldChanged;
			_issuesEventsBuffer.OnIssueCreateEventReceived += OnIssueEventsCreated;
			_issuesEventsBuffer.OnIssueLinksAddedEventReceived += OnIssueEventsLinksAdded;
			_issuesEventsBuffer.OnIssueLinksRemovedEventReceived += OnIssueEventsLinksRemoved;
			_issuesEventsBuffer.OnIssueCommentsAddedEventReceived += OnIssueEventsCommentsAdded;
			_issuesEventsBuffer.OnIssueStateBecomeTestReceived += OnIssueStateBecomeTestReceived;
		}

		private void OnIssueEventsAssigneeChanged(YouTrackIssueAssigneeChangedEvent eventData)
		{
			string newUserLogin = eventData.NewValue.Login;
			string oldUserLogin = eventData.OldValue.Login;
			string updaterUserLogin = eventData.Updater.Login;

			bool isNeedSendMessageNewUser = newUserLogin != updaterUserLogin;
			bool isNeedSendMessageOldUser = oldUserLogin != updaterUserLogin;

			if (isNeedSendMessageNewUser)
			{
				SendYouTrackNotification(newUserLogin, GetAssignedText(eventData), YouTrackNotificationType.Assigned);
			}

			if(isNeedSendMessageOldUser)
			{
				SendYouTrackNotification(oldUserLogin, GetReassignedText(eventData), YouTrackNotificationType.Reassigned);
			}
		}

		private void OnIssueEventsSummaryChanged(YouTrackIssueStringValueChangedEvent eventData)
		{
			SendYouTrackNotification(eventData.Issue.Assignee.Login, GetUpdatedSummaryText(eventData), YouTrackNotificationType.UpdatedSummary);
		}

		private void OnIssueEventsDescriptionChanged(YouTrackIssueStringValueChangedEvent eventData)
		{
			SendYouTrackNotification(eventData.Issue.Assignee.Login, GetUpdatedDescriptionText(eventData), YouTrackNotificationType.UpdatedDescription);
		}

		private void OnIssueEventsSimpleFieldChanged(YouTrackIssueSimpleFieldChangedEvent eventData)
		{
			if (eventData.FieldName == "State")
			{
				SendYouTrackNotification(eventData.Issue.Assignee.Login, GetUpdatedFieldText(eventData), YouTrackNotificationType.UpdatedState);
			}
			else if(eventData.FieldName == "Priority")
			{
				SendYouTrackNotification(eventData.Issue.Assignee.Login, GetUpdatedFieldText(eventData), YouTrackNotificationType.UpdatedPriority);
			}
			else
			{
				SendYouTrackNotification(eventData.Issue.Assignee.Login, GetUpdatedFieldText(eventData), YouTrackNotificationType.UpdatedOtherField);
			}
		}

		private void OnIssueEventsCreated(YouTrackIssueCreateEvent eventData)
		{
			SendYouTrackNotification(eventData.Assignee.Login, GetCreatedText(eventData), YouTrackNotificationType.Created);
		}

		private void OnIssueEventsLinksAdded(YouTrackIssueLinksEvent eventData)
		{
			List<YouTrackIssueLink> links = eventData.Links;
			for (int i = 0; i < links.Count; i++)
			{
				YouTrackIssueLink link = links[i];
				SendYouTrackNotification(eventData.Issue.Assignee.Login, GetLinkAddedText(link, eventData.Issue, eventData.Updater), YouTrackNotificationType.LinkAdded);
			}
		}

		private void OnIssueEventsLinksRemoved(YouTrackIssueLinksEvent eventData)
		{
			List<YouTrackIssueLink> links = eventData.Links;
			for (int i = 0; i < links.Count; i++)
			{
				YouTrackIssueLink link = links[i];
				SendYouTrackNotification(eventData.Issue.Assignee.Login, GetLinkRemovedText(link, eventData.Issue, eventData.Updater), YouTrackNotificationType.LinkRemoved);
			}
		}

		private void OnIssueEventsCommentsAdded(YouTrackIssueCommentsEvent eventData)
		{
			for (int i = 0; i < eventData.Comments.Count; i++)
			{
				YouTrackIssueComment comment = eventData.Comments[i];
				if (comment.IsActual(eventData.Issue))
				{
					if(comment.IsActualForAssignee(eventData.Issue))
					{
						SendYouTrackNotification(eventData.Issue.Assignee.Login, GetCommentedText(comment, eventData.Issue), YouTrackNotificationType.Commented);
					}

					List<string> actualMentionedUsers;
					if(comment.IsActualForMentions(eventData.Issue, out actualMentionedUsers))
					{
						SendUserMentions(eventData.Issue, comment, actualMentionedUsers);
					}
				}
			}
		}

		private void OnIssueStateBecomeTestReceived(YouTrackIssueSimpleFieldChangedEvent eventData)
		{
			SendTesterNotification(GetReadyForTestText(eventData.Issue), eventData.Updater.Login);
		}

		private void SendUserMentions(YouTrackIssue issue, YouTrackIssueComment comment, List<string> actualMentionedUsers)
		{
			for (int i = 0; i < actualMentionedUsers.Count; i++)
			{
				string mentionedUser = actualMentionedUsers[i];
				SendYouTrackNotification(mentionedUser, GetMentionedText(issue, comment), YouTrackNotificationType.Mentioned);
			}
		}

		private string GetAssignedText(YouTrackIssueAssigneeChangedEvent eventData)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, eventData.Issue, "Assigned");

			builder.AppendLine($"From: {eventData.Updater.FullName}".ToItalic());
			builder.Append($"Assignee: {eventData.OldValue.FullName} {Symbols.ArrowRight} You".ToItalic());

			return builder.ToString();
		}

		private string GetReassignedText(YouTrackIssueAssigneeChangedEvent eventData)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, eventData.Issue, "Reassigned");

			builder.AppendLine($"From: {eventData.Updater.FullName}".ToItalic());
			builder.Append($"Assignee: You {Symbols.ArrowRight} {eventData.NewValue.FullName}".ToItalic());

			return builder.ToString();
		}

		private string GetUpdatedSummaryText(YouTrackIssueStringValueChangedEvent eventData)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, eventData.Issue, "Updated");

			builder.AppendLine($"From: {eventData.Updater.FullName}".ToItalic());
			builder.Append($"Summary: ...see notification header...".ToItalic());

			return builder.ToString();
		}

		private string GetUpdatedDescriptionText(YouTrackIssueStringValueChangedEvent eventData)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, eventData.Issue, "Updated");

			builder.AppendLine($"From: {eventData.Updater.FullName}".ToItalic());
			builder.Append($"Description: ".ToItalic());
			builder.Append($"...open issue for see...".ToLink(eventData.Issue.Url));

			return builder.ToString();
		}

		private string GetUpdatedFieldText(YouTrackIssueSimpleFieldChangedEvent eventData)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, eventData.Issue, "Updated");

			builder.AppendLine($"From: {eventData.Updater.FullName}".ToItalic());

			string oldValue = string.IsNullOrEmpty(eventData.OldValue) ? "none" : eventData.OldValue;
			string newValue = string.IsNullOrEmpty(eventData.NewValue) ? "none" : eventData.NewValue;
			builder.Append($"{eventData.FieldName}: {oldValue} {Symbols.ArrowRight} {newValue}".ToItalic());

			return builder.ToString();
		}

		private string GetCreatedText(YouTrackIssueCreateEvent eventData)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, eventData.Issue, "Created");

			builder.AppendLine($"From: {eventData.Updater.FullName}".ToItalic());
			builder.Append($"State: {eventData.Issue.State}".ToItalic());

			return builder.ToString();
		}

		private string GetLinkAddedText(YouTrackIssueLink link, YouTrackIssue issue, YouTrackUser updater)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, issue, "Link added");

			builder.AppendLine($"From: {updater.FullName}".ToItalic());
			builder.Append($"Link: added {link.LinkType} ".ToItalic());
			builder.Append($"{link.Issue.Link}");

			return builder.ToString();
		}

		private string GetLinkRemovedText(YouTrackIssueLink link, YouTrackIssue issue, YouTrackUser updater)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, issue, "Link removed");

			builder.AppendLine($"From: {updater.FullName}".ToItalic());
			builder.Append($"Link: removed {link.LinkType} ".ToItalic());
			builder.Append($"{link.Issue.Link}");

			return builder.ToString();
		}

		private string GetCommentedText(YouTrackIssueComment comment, YouTrackIssue issue)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, issue, "Commented");

			builder.AppendLine($"{comment.Author.FullName}:".ToItalic());
			builder.Append(comment.Text.ToBlockCode());

			return builder.ToString();
		}

		private string GetMentionedText(YouTrackIssue issue, YouTrackIssueComment comment)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, issue, "Mentioned");

			builder.AppendLine($"{comment.Author.FullName}:".ToItalic());
			builder.Append(comment.Text.ToBlockCode());

			return builder.ToString();
		}

		private string GetReadyForTestText(YouTrackIssue issue)
		{
			StringBuilder builder = new StringBuilder();
			AppendHeader(builder, issue, "Test");

			builder.AppendLine($"From: {issue.Assignee.FullName}".ToItalic());

			if (!String.IsNullOrEmpty(issue.Version))
			{
				builder.AppendLine($"Version: {issue.Version}".ToItalic());
			}

			if (!String.IsNullOrEmpty(issue.Build))
			{
				builder.AppendLine($"Build: {issue.Build}".ToItalic());
			}

			return builder.ToString();
		}

		private void AppendHeader(StringBuilder builder, YouTrackIssue issue, string actionText)
		{
			string priority = TelegramUtilities.GetPriorityEmoji(issue.Priority);
			string issueLink = issue.LinkWithBrackets;
			string action = $"[{actionText.ToItalic()}]";
			builder.AppendLine($"{priority}{issueLink}{action} {issue.Summary.ToBold()}");
			builder.AppendLine();
		}

		//TODO: return success send message chatId[]
		private void SendYouTrackNotification(string login, string message, YouTrackNotificationType notificationType)
		{
			List<LinkedUser> linkedUsers;
			if (_database.TryGetLinkedUsersByYoutrackLogin(login, out linkedUsers))
			{
				for (int i = 0; i < linkedUsers.Count; i++)
				{
					LinkedUser linkedUser = linkedUsers[i];

					if (linkedUser.IsNotificationEnabled(notificationType))
					{
						_telegram.SendYouTrackNotification(linkedUser.TelegramChatId, message);
					}
				}
			}
		}

		private void SendTesterNotification(string message, string updaterLogin)
		{
			List<LinkedUser> allLinkedUsers = _database.GetAllLinkedUsers();
			for (int i = 0; i < allLinkedUsers.Count; i++)
			{
				LinkedUser linkedUser = allLinkedUsers[i];
				if (linkedUser.IsTester && linkedUser.YouTrackLogin != updaterLogin)
				{
					_telegram.SendYouTrackNotification(linkedUser.TelegramChatId, message);
				}
			}
		}
	}
}