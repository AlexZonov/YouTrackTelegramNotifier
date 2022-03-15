using System.Text.RegularExpressions;

namespace com.services.telegram.dialogs
{
	public class ChoseNotificationResponseHandler
	{
		public ChooseNotificationRecord Record { get; private set; }

		public ChoseNotificationResponseHandler(string responseText, ChooseNotificationRequest request)
		{
			Regex regex = new Regex(@"^(?<id>\d+)\.\s");
			Match match = regex.Match(responseText);

			if (match.Success)
			{
				string responseId = match.Groups["id"].Value;

				ChooseNotificationRecord chooseRecord;
				if (request.Records.TryGetValue(responseId, out chooseRecord))
				{
					Record = chooseRecord;
				}
			}
		}
	}
}