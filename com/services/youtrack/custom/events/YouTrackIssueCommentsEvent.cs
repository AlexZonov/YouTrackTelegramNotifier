using System.Collections.Generic;
using com.services.youtrack.custom.issue;
using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueCommentsEvent : YouTrackIssueEventBase
	{
		[JsonProperty("comments")]
		public List<YouTrackIssueComment> Comments { get; private set; }

		protected override void StackImpl(YouTrackIssueEventBase otherEvent)
		{
			YouTrackIssueCommentsEvent similarEvent = (YouTrackIssueCommentsEvent) otherEvent;
			for (int i = 0; i < similarEvent.Comments.Count; i++)
			{
				YouTrackIssueComment similarComment = similarEvent.Comments[i];
				if (!IsContainsComment(similarComment))
				{
					Comments.Add(similarComment);
				}
			}
		}

		protected override bool IsActualImpl()
		{
			for (int i = 0; i < Comments.Count; i++)
			{
				YouTrackIssueComment comment = Comments[i];

				if(comment.IsActual(Issue))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsContainsComment(YouTrackIssueComment similarComment)
		{
			for (int i = 0; i < Comments.Count; i++)
			{
				YouTrackIssueComment comment = Comments[i];
				if (comment.Author.Login == similarComment.Author.Login && comment.Text == similarComment.Text)
				{
					return true;
				}
			}

			return false;
		}
	}
}