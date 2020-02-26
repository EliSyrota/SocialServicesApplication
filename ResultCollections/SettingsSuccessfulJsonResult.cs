namespace SocialServicesApplication
{
	public class SettingsSuccessfulJsonResult : JsonResultBase<object>
	{
		public SettingsSuccessfulJsonResult()
			: base(201, false, "Settings changed successful")
		{
		}
	}
}