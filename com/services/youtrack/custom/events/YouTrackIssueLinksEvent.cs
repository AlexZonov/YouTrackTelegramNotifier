using System.Collections.Generic;
using com.services.youtrack.custom.issue;
using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class YouTrackIssueLinksEvent : YouTrackIssueEventBase
	{
		[JsonProperty("updater")]
		public YouTrackUser Updater { get; private set; }

		[JsonProperty("links")]
		public List<YouTrackIssueLink> Links { get; private set; }

		protected override void StackImpl(YouTrackIssueEventBase otherEvent)
		{
			YouTrackIssueLinksEvent similarEvent = (YouTrackIssueLinksEvent) otherEvent;
			Updater = similarEvent.Updater;

			for (int i = 0; i < similarEvent.Links.Count; i++)
			{
				YouTrackIssueLink similarLink = similarEvent.Links[i];
				if (!IsContainsLink(similarLink))
				{
					Links.Add(similarLink);
				}
			}
		}

		protected override bool IsActualImpl()
		{
			return Updater.Login != Issue.Assignee.Login && Links.Count > 0;
		}

		private bool IsContainsLink(YouTrackIssueLink similarLink)
		{
			for (int i = 0; i < Links.Count; i++)
			{
				YouTrackIssueLink link = Links[i];
				if (link.LinkType == similarLink.LinkType && link.Issue.Id == similarLink.Issue.Id)
				{
					return true;
				}
			}

			return false;
		}
	}
}