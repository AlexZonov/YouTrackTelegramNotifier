/**
 * This is a template for a custom script. Here you can add any
 * functions and objects you want to. To use these functions and objects in other
 * scripts, add them to the 'exports' object.
 * Use the following syntax to reference this script:
 *
 * `var myScript = require('./this-script-name');`
 *
 * For details, read the Quick Start Guide:
 * https://www.jetbrains.com/help/youtrack/standalone/2018.3/Quick-Start-Guide-Workflows-JS.html
 */

var entities = require('@jetbrains/youtrack-scripting-api/entities');
var workflow = require('@jetbrains/youtrack-scripting-api/workflow');
var telegram_sender_namespace = require('telegram_notifier_v2/telegram_bot_sender');

var telegramSender = new telegram_sender_namespace.TelegramSender();

exports.rule = entities.Issue.onChange(
{
	title: workflow.i18n('Send notification to Telegram when an issue is changed or commented'),
	action: function(ctx)
	{
		var issue = ctx.issue;
		if(issue === null || issue === undefined)
		{
			return;
		}

		var updater = ctx.currentUser;
		var assigne = issue.fields.Assignee;
		if(assigne === null)
		{
			return;
		}

		telegramSender.trySendIssueCreatedEvent(updater, issue);

		telegramSender.trySendIssueSummaryChangedEvent(updater, issue);
		telegramSender.trySendIssueDescriptionChangedEvent(updater, issue);
		telegramSender.trySendAssigneeChangedEvent(updater, issue);

		telegramSender.trySendIssueAddCommentsEvent(issue);

		var projectFields = issue.project.fields;
		projectFields.forEach(function(projectField)
		{
			var isSupportedField = telegramSender.EXCLUDE_SIMPLE_FIELDS.indexOf(projectField.name) === -1;
			if(isSupportedField)
			{
				telegramSender.trySendSimpleFieldChangedEvent(updater, issue, projectField.name);
			}
		});

		try
		{
			telegramSender.trySendIssueAddLinksEvent(updater, issue);
			telegramSender.trySendIssueRemoveLinksEvent(updater, issue);
		}
		catch (err)
		{
			console.log("Error: " + err + "\r\n" + err.stack);
		}
	}
});