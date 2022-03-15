using System;
using System.Collections.Generic;
using com.database;
using com.services.telegram.youtrack;
using com.utilities;

namespace com.services.telegram.dialogs
{
	public class ChooseNotificationRequest
	{
		public bool IsEmpty { get; private set; } = true;
		public Dictionary<string, ChooseNotificationRecord> Records { get; private set; }

		public ChooseNotificationRequest(LinkedUser linkedUser, bool isDisableMode)
		{
			Records = new Dictionary<string, ChooseNotificationRecord>();

			int[] typesValues = (int[]) Enum.GetValues(typeof(YouTrackNotificationType));
			for (int i = 0; i < typesValues.Length; i++)
			{
				YouTrackNotificationType typeValue = (YouTrackNotificationType) typesValues[i];
				if (linkedUser.IsNotificationEnabled(typeValue) == isDisableMode)
				{
					string id = (Records.Count + 1).ToString();
					IsEmpty = false;
					Records[id] = new ChooseNotificationRecord(id, typeValue, StringUtilities.GetVariablePresentation(typeValue.ToString(), true));
				}
			}
		}
	}
}