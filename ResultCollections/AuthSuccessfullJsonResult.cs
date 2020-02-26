namespace SocialServicesApplication
{
	public class AuthSuccessfulJsonResult : JsonResultBase<object>
	{
		public AuthSuccessfulJsonResult()
			: base(200, false, "Auth successful")
		{
		}
	}
}