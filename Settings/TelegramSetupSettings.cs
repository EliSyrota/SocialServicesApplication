namespace SocialServicesApplication
{
	public class TelegramSetupSettings : IServiceSettings
	{
		public string ApiHash { get; set; }
		public string ApiId { get; set; }
		public string NumberToAuthenticate { get; set; }
		public string CodeToAuthenticate { get; set; }
	}
}