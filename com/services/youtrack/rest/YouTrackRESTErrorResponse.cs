using System.Text;
using Newtonsoft.Json;

namespace com.services.youtrack.rest
{
	public struct YouTrackRESTErrorResponse
	{
		[JsonProperty("value")]
		public string Value { get; private set; }

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("Value: ");
			builder.AppendLine(Value);

			return builder.ToString();
		}
	}
}