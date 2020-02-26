using System;

namespace SocialServicesApplication
{
	[Serializable]
	public class Lead : ILead
	{
		public int Id { get; set; }
		public string Phone { get; set; }
		public bool IsViber { get; set; }
		public bool IsTelegram { get; set; }
		public string TelegramUser { get; set; }
		public bool IsWhatsApp { get; set; }

		public Lead(ILead lead)
		{
			Id = lead.Id;
			Phone = lead.Phone;
		}
	}
}