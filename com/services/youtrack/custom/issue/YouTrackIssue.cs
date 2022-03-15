using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace com.services.youtrack.custom.issue
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssue : YouTrackIssueBase
	{
		[JsonProperty("description")]
		public string Description { get; private set; }

		//[JsonProperty("assignee_login")]
		//public string AssigneeLogin { get; private set; }

		[JsonProperty("assignee")]
		public YouTrackUser Assignee { get; private set; }

		[JsonProperty("fields")]//[JsonDictionaryConverter()] https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-converters-how-to
		//TODO: to dictionary
		public List<YouTrackIssueField> Fields { get; private set; }

		public string State { get; private set; }
		public string Build { get; private set; }
		public string Version { get; private set; }
		public YouTrackIssuePriority Priority { get; private set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			State = GetFieldValueByName("State");
			Build = GetFieldValueByName("Build");
			Version = GetFieldValueByName("Version");

			Enum.TryParse<YouTrackIssuePriority>(GetFieldValueByName("Priority"), true, out YouTrackIssuePriority priority);
			Priority = priority;
		}

		private string GetFieldValueByName(string name)
		{
			return Fields.FirstOrDefault(field => field.Name == name)?.Value;
		}
	}
}