using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace com.services.youtrack.custom.issue
{
	public enum YouTrackIssuePriority
	{
		Undefined = -1,
		Low,
		Normal,
		High,
		Critical
	}
}