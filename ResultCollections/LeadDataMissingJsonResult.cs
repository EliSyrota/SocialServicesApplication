namespace SocialServicesApplication
{
	public class LeadDataMissingJsonResult : JsonResultBase<object>
	{
		public LeadDataMissingJsonResult()
			: base(402, true, "Lead information is missing in query")
		{
		}
	}
}