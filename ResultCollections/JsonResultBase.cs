using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SocialServicesApplication
{
	public class JsonResultBase<T>
	{
		public int ResultCode { get; }
		public bool IsException { get; }
		public string MessageText { get; }
		public Dictionary<string, T> Values { get; protected set; }

		public JsonResultBase(int resultCode, bool isException, string messageText, Dictionary<string, T> values = null)
		{
			ResultCode = resultCode;
			IsException = isException;
			MessageText = messageText;
			Values = values;
		}
	}
}