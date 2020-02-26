using System;

namespace SocialServicesApplication
{
	[Serializable]
	public class LeadRequest : ILead
	{
		public int Id { get; set; }
		public string Phone { get; set; }
	}
}