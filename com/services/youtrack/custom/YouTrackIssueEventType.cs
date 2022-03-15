namespace com.services.youtrack.custom.events
{
	public enum YouTrackIssueEventType
	{
		AssigneeChanged = 0,
		SummaryChanged = 1,
		DescriptionChanged = 2,
		SimpleFieldChanged = 3,
		Created = 4,
		LinksAdded = 5,
		LinksRemoved = 6,
		CommentsAdded = 7
	}
}