using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace com.services.youtrack.custom.issue
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueComment
	{
		[JsonProperty("author")]
		public YouTrackUser Author { get; private set; }

		[JsonProperty("text")]
		public string Text { get; private set; }

		[JsonProperty("url")]
		public string Url { get; private set; }

		public List<string> MentionedUsers { get; private set; }

		public bool IsActual(YouTrackIssue issue)
		{
			if (IsActualForAssignee(issue))
			{
				return true;
			}

			if(IsSomeMentionActual(issue))
			{
				return true;
			}

			return false;
		}

		public bool IsActualForAssignee(YouTrackIssue issue)
		{
			if (Author.Login != issue.Assignee.Login)
			{
				return true;
			}

			return false;
		}

		public bool IsActualForMentions(YouTrackIssue issue, out List<string> actualMentionedUsers)
		{
			actualMentionedUsers = GetActualMentionedUsers(issue);
			return actualMentionedUsers.Count > 0;
		}

		private bool IsSomeMentionActual(YouTrackIssue issue)
		{
			for(int i = 0; i < MentionedUsers.Count; i++)
			{
				string mentionedUser = MentionedUsers[i];
				if(IsMentionActual(issue, mentionedUser))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsMentionActual(YouTrackIssue issue, string mentionedUser)
		{
			if(mentionedUser != Author.Login && mentionedUser != issue.Assignee.Login)
			{
				return true;
			}

			return false;
		}

		private List<string> GetActualMentionedUsers(YouTrackIssue issue)
		{
			List<string> result = new List<string>();

			for(int j = 0; j < MentionedUsers.Count; j++)
			{
				string mentionedUser = MentionedUsers[j];
				if(IsMentionActual(issue, mentionedUser))
				{
					result.Add(mentionedUser);
				}
			}

			return result;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			MentionedUsers = GetUserMentions(Text);
		}

		private List<string> GetUserMentions(string text)
		{
			string regexPattern = "@([^@, \"]{1,50})";
			Regex regex = new Regex(regexPattern, RegexOptions.Multiline);
			MatchCollection matches = regex.Matches(text);
			return matches.Select(match => match.Groups[1].Value).ToList();
		}
	}
}