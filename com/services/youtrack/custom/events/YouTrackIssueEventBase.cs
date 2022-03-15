using System.Runtime.Serialization;
using com.services.youtrack.custom.issue;
using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class YouTrackIssueEventBase
	{
		[JsonProperty("issue")]
		public YouTrackIssue Issue { get; private set; }

		public bool IsActual { get; private set; }

		public virtual bool IsSimilar(YouTrackIssueEventBase otherEvent)
		{
			return this.GetType() == otherEvent.GetType();
		}

		public void Stack(YouTrackIssueEventBase otherEvent)
		{
			StackImpl(otherEvent);
		}

		public void UpdateIssue(YouTrackIssue issue)
		{
			Issue = issue;
			OnDeserializeImpl();
		}

		[OnDeserialized]
		private void OnDeserialize(StreamingContext context)
		{
			OnDeserializeImpl();
		}

		private void OnDeserializeImpl()
		{
			IsActual = IsActualImpl();
		}

		protected abstract void StackImpl(YouTrackIssueEventBase otherEvent);
		protected abstract bool IsActualImpl();
	}
}