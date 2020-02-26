namespace SocialServicesApplication
{
	public class TelegramTwoWayAuthPerformingResult : JsonResultBase<object>
	{
		public TelegramTwoWayAuthPerformingResult() : base(403, false, "waiting for two-way auth: send password from telegram client")
		{
		}
	}
}