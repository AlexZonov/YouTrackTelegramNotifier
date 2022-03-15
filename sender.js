/**
 * @module telegram_notification_v2/sender
 */

this.EXCLUDE_SIMPLE_FIELDS = ["Assignee"];

var WEBHOOK_URL = "http://address:port/web_server_url_path/";
var ACCESS_TOKEN = "";
var http = require('@jetbrains/youtrack-scripting-api/http');

var EVENT_ISSUE_ASSIGNEE_CHANGED = 0;
var EVENT_ISSUE_SUMMARY_CHANGED = 1;
var EVENT_ISSUE_DESCRIPTION_CHANGED = 2;
var EVENT_ISSUE_SIMPLE_FIELD_CHANGED = 3;
var EVENT_ISSUE_CREATED = 4;
var EVENT_ISSUE_ADDED_LINKS = 5;
var EVENT_ISSUE_REMOVED_LINKS = 6;
var EVENT_ISSUE_ADDED_COMMENTS = 7;

var LINKS_TYPES = [
    "relates to",
    "duplicates",
    "is duplicated by",
    "depends on",
    "is required for",
    "subtask of",
    "parent for"
];

var TelegramSender = function()
{
    //empty
};

TelegramSender.prototype.EXCLUDE_SIMPLE_FIELDS = ["Assignee"];

TelegramSender.prototype.trySendSimpleFieldChangedEvent = function(updater, issue, fieldName)
{
    if(!issue.isReported)
    {
        return;
    }

    var fields = issue.fields;
    var projectFields = issue.project.fields;
    var projectField = projectFields.find(function(f) { return f.name === fieldName; });

    if(projectField === null || projectField === undefined)
    {
        //console.log("Field: " + fieldName + " is " + projectField);
        return;
    }

    if(!fields.isChanged(fieldName))
    {
        return;
    }

    var oldValue = fields.oldValue(fieldName);
    var newValue = fields[fieldName];

    var struct =
    {
        "issue": getIssueShortJson(issue),
        "updater": getUserJson(updater),
        "field": fieldName,
        "old_value": oldValue !== null ? oldValue.presentation : "",
        "new_value": newValue !== null ? newValue.presentation : ""
    };
    sendEvent(EVENT_ISSUE_SIMPLE_FIELD_CHANGED, struct);
};

TelegramSender.prototype.trySendAssigneeChangedEvent = function(updater, issue)
{
    trySendIssueChangedEvent(updater, issue, "Assignee", EVENT_ISSUE_ASSIGNEE_CHANGED, userWrapValueMethod);
};

TelegramSender.prototype.trySendIssueSummaryChangedEvent = function(updater, issue)
{
    trySendIssueChangedEvent(updater, issue, "summary", EVENT_ISSUE_SUMMARY_CHANGED, stringWrapValueMethod);
};

TelegramSender.prototype.trySendIssueDescriptionChangedEvent = function(updater, issue)
{
    trySendIssueChangedEvent(updater, issue, "description", EVENT_ISSUE_DESCRIPTION_CHANGED, stringWrapValueMethod);
};

TelegramSender.prototype.trySendIssueCreatedEvent = function(updater, issue)
{
    if(!issue.becomesReported)
    {
        return;
    }

    var struct =
    {
        "issue": getIssueShortJson(issue),
        "updater": getUserJson(updater),
        "assignee": getUserJson(issue.Assignee)
    };
    sendEvent(EVENT_ISSUE_CREATED, struct);
};

TelegramSender.prototype.trySendIssueAddCommentsEvent = function(issue)
{
    if(!issue.isReported || issue.comments.added.isEmpty())
    {
        return;
    }

    var struct =
    {
        "issue": getIssueShortJson(issue),
        "comments": getCommentsJson(issue.comments.added)
    };
    sendEvent(EVENT_ISSUE_ADDED_COMMENTS, struct);
};

TelegramSender.prototype.trySendIssueAddLinksEvent = function(updater, issue)
{
    var linkedIssues = getAllAddedLinkedIssues(issue);
    if(linkedIssues === null)
    {
        return;
    }

    var struct =
    {
        "issue": getIssueShortJson(issue),
        "links": linkedIssues,
        "updater": getUserJson(updater)
    };
    sendEvent(EVENT_ISSUE_ADDED_LINKS, struct);
};

TelegramSender.prototype.trySendIssueRemoveLinksEvent = function(updater, issue)
{
    var linkedIssues = getAllRemovedLinkedIssues(issue);
    if(linkedIssues === null)
    {
        return;
    }

    var struct =
    {
        "issue": getIssueShortJson(issue),
        "links": linkedIssues,
        "updater": getUserJson(updater)
    };
    sendEvent(EVENT_ISSUE_REMOVED_LINKS, struct);
};

var trySendIssueChangedEvent = function(updater, issue, fieldName, eventId, wrapValueMethod)
{
    if(!issue.isReported)
    {
        return;
    }

    if(!issue.isChanged(fieldName))
    {
        return;
    }

    var oldValue = issue.oldValue(fieldName);
    var newValue = issue[fieldName];

    var struct =
    {
        "issue": getIssueShortJson(issue),
        "updater": getUserJson(updater),
        "old_value": wrapValueMethod(oldValue),
        "new_value": wrapValueMethod(newValue)
    };
    sendEvent(eventId, struct);
};

var getAllAddedLinkedIssues = function(issue)
{
    return getAllLinkedIssuesWithFilter(issue, linksAddedGetter);
};

var getAllRemovedLinkedIssues = function(issue)
{
    return getAllLinkedIssuesWithFilter(issue, linksRemovedGetter);
};

var getAllLinkedIssuesWithFilter = function(issue, linksAdditionalGetter)
{
    if(!issue.isReported)
    {
        return null;
    }
    
    //console.log(issue);

    var struct = [];
    for(var i = 0 ; i < LINKS_TYPES.length ; i++)
    {
        var linksType = LINKS_TYPES[i];
        var linkedIssues = getLinkedIssuesWithFilter(issue, linksAdditionalGetter, linksType);
        for (var j = 0 ; j < linkedIssues.length ; j++)
        {
            struct.push(linkedIssues[j]);
        }
    }

    if(struct.length > 0)
    {
        return struct;
    }
    else
    {
        return null;
    }
};

var getLinkedIssuesWithFilter = function(issue, linksAdditionalGetter, linkTypeName)
{
    var result = [];

    var links = issue.links;
    var linkedIssues = links[linkTypeName];

    if(linkedIssues !== null && linkedIssues !== undefined)
    {
        var gettedLinks = linksAdditionalGetter(linkedIssues);
        if(gettedLinks.isNotEmpty())
        {
            gettedLinks.forEach(function(linkedIssue)
            {
                //if(!linkedIssue.isReported)
                //{
                //    console.log("\r\n\r\n not reported issue:" + linkedIssue);
                //    return;
                //}

                var issueLink = getIssueLinkJson(linkedIssue);
                var struct =
                {
                    "link_type": linkTypeName,
                    "issue_link": issueLink
                };
                result.push(struct);
            });
        }
    }

    return result;
};

var linksAddedGetter = function(linkedIssues)
{
    return linkedIssues.added;
};

var linksRemovedGetter = function(linkedIssues)
{
    return linkedIssues.removed;
};

var stringWrapValueMethod = function(value)
{
    return value;
};

var userWrapValueMethod = function(value)
{
    return getUserJson(value);
};

var getIssueShortJson = function(issue)
{
    var struct = getIssueLinkJson(issue);
    struct["description"] = issue.description;
    struct["assignee"] = getUserJson(issue.Assignee);
    struct["fields"] = getSimpleProjectFields(issue);
    return struct;
};

var getIssueLinkJson = function(issue)
{
    var struct =
    {
        "id": issue.id,
        "url": issue.url,
        "summary": issue.summary,
        "resolved": issue.isResolved
    };
    return struct;
};

var getSimpleProjectFields = function(issue)
{
    var result = [];
    var projectFields = issue.project.fields;

    projectFields.forEach(function(projectField)
    {
        var projectFieldName = projectField.name;
        var isSupportedField = this.EXCLUDE_SIMPLE_FIELDS.indexOf(projectFieldName) === -1;
        if(isSupportedField)
        {
            var isFieldVisible = projectField.isVisibleInIssue(issue);
            var fieldValue = isFieldVisible ? projectField.getValuePresentation(issue) : "";
            var field_struct =
            {
                "name": projectFieldName,
                "value": fieldValue
            };
            result.push(field_struct);
        }
    });

    return result;
};

var getCommentsJson = function(comments)
{
    var struct = [];
    comments.forEach(function(comment)
    {
        struct.push(getCommentJson(comment));
    });
    return struct;
};

var getCommentJson = function(comment)
{
    var struct =
    {
        "author": getUserJson(comment.author),
        "text": comment.text,
        "url": comment.url
    };
    return struct;
};

var getUserJson = function(user)
{
    var struct =
    {
        "login": user !== null ? user.login : "Unassigned",
        "full_name": user !== null ? user.fullName : "Unassigned",
        "email": user !== null ? user.email : ""
    };
    return struct;
};

var sendEvent = function(event_id, data)
{
    var struct =
    {
        "event": event_id,
        "data": data
    };
    sendRequest(struct);
};

var sendRequest = function(struct)
{
    var connection = new http.Connection(WEBHOOK_URL, null, 20000);
    connection.addHeader("Content-Type", "application/json");
    connection.addHeader("Access-Token", ACCESS_TOKEN);

    var json = JSON.stringify(struct);
    //console.log(json);
    var response = connection.postSync('', null, json);
    if (!response.isSuccess)
    {
        console.warn('Failed to post notification to Telegram. Details: ' + response.toString());
    }
    return this;
};

exports.TelegramSender = TelegramSender;