using System;
using System.Text;

namespace com.utilities
{
	internal static class TypeUtilities
	{
		public static string GetNiceName(this Type type)
		{
			return GetNiceNameImpl(type, false);
		}

		public static string GetNiceFullName(this Type type)
		{
			return GetNiceNameImpl(type, true);
		}

		private static string GetNiceNameImpl(this Type type, bool isFull = false)
		{
			string result;
			StringBuilder retType = new StringBuilder();

			if (type.IsGenericType)
			{
				string parentType = type.FullName.Split('`')[0];
				if (!isFull)
				{
					string[] dotDelimeterParts = parentType.Split('.');
					parentType = dotDelimeterParts[dotDelimeterParts.Length - 1];
				}

				Type[] arguments = type.GetGenericArguments();
				StringBuilder argList = new StringBuilder();
				foreach (Type t in arguments)
				{
					string arg = t.GetNiceNameImpl(isFull);
					if (argList.Length > 0)
					{
						argList.AppendFormat(", {0}", arg);
					}
					else
					{
						argList.Append(arg);
					}
				}
 
				if (argList.Length > 0)
				{
					retType.AppendFormat("{0}<{1}>", parentType, argList.ToString());
				}

				result = retType.ToString();
			}
			else
			{
				if (!isFull)
				{
					string[] dotDelimeterParts = type.ToString().Split('.');
					result = dotDelimeterParts[dotDelimeterParts.Length - 1];
				}
				else
				{
					result = type.ToString();
				}
			}
 
			return result.Replace('+','.');
		}
	}
}