using System.Text;
using Newtonsoft.Json;

namespace com.services.youtrack.rest
{
	public class YouTrackRESTUser
	{
		[JsonProperty("login")]
		public string Login { get; private set; }

		[JsonProperty("fullName")]
		public string FullName { get; private set; }

		[JsonProperty("email")]
		public string Email { get; private set; }

		[JsonProperty("jabber")]
		public string Jabber { get; private set; }

		[JsonProperty("ringId")]
		public string RingId { get; private set; }

		[JsonProperty("groupsUrl")]
		public string GroupsUrl { get; private set; }

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("Login: ");
			builder.AppendLine(Login);

			builder.Append("FullName: ");
			builder.AppendLine(FullName);

			builder.Append("Email: ");
			builder.AppendLine(Email);

			builder.Append("Jabber: ");
			builder.AppendLine(Jabber);

			builder.Append("RingId: ");
			builder.AppendLine(RingId);

			builder.Append("GroupsUrl: ");
			builder.AppendLine(GroupsUrl);

			return builder.ToString();
		}
	}
}