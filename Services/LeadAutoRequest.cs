using System.Collections.Generic;

namespace SocialServicesApplication
{
	public class LeadAutoRequest
	{
		public int PageRange { get; set; }
		public int CurrentPageNumber { get; set; }
		public int Pages { get; set; }
		public List<Item> Items { get; set; }
	}
}