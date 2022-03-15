using System.IO;
using Newtonsoft.Json;
using File = System.IO.File;

namespace com.utilities
{
	internal static class JsonFilesUtilities
	{
		public static void Save(string fileName, object data)
		{
			TryCreateFile(fileName);
			File.WriteAllText(fileName, JsonConvert.SerializeObject(data, Formatting.Indented));
		}

		public static T Load<T>(string fileName)
		{
			string jsonData = ReadFromFile(fileName);
			return JsonConvert.DeserializeObject<T>(jsonData);
		}

		public static void Drop(string fileName)
		{
			if(File.Exists(fileName))
			{
				File.Delete(fileName);
			}
		}

		private static string ReadFromFile(string fileName)
		{
			TryCreateFile(fileName);
			string jsonData = File.ReadAllText(fileName);
			return jsonData;
		}

		private static void TryCreateFile(string fileName)
		{
			if(!File.Exists(fileName))
			{
				FileStream fileStream = File.Create(fileName);
				fileStream.Close();
			}
		}
	}
}