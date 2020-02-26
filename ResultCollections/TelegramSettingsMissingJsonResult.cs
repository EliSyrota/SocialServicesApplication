using System.Collections.Generic;

namespace SocialServicesApplication
{
	public class TelegramSettingsMissingJsonResult : JsonResultBase<object>
	{
		public TelegramSettingsMissingJsonResult(IServiceSettings telegramSettings)
			: base(401, true, $"Wrong Telegram settings or your session was terminated. Please set up ApiId, ApiHash, Phone and auth code")
		{
			Values = new Dictionary<string, object>()
			{
				{ "ApiId", telegramSettings?.ApiId },
				{ "ApiHash", telegramSettings?.ApiHash },
				{ "NumberToAuthenticate", telegramSettings?.NumberToAuthenticate },
				{ "CodeToAuthentificate", "Hidden for security reasons" }
			};
		}
	}
}