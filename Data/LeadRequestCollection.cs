using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialServicesApplication
{
	[Serializable]
	public class LeadCollection
	{
		public LeadCollection(List<LeadRequest> leads)
		{
			Leads = new Dictionary<string, Lead>();
			foreach (var lead in leads)
			{
				lead.Phone = lead.Phone.Replace("+", "");
				Leads[lead.Phone] = new Lead(lead);
			}
		}

		public bool TryGetLead(string phone, out Lead lead)
		{
			lead = null;
			return Leads.TryGetValue(phone, out lead);
		}

		public List<Lead> GetLeadsList()
		{
			return Leads.Values.ToList();
		}

		public Dictionary<string, Lead> Leads { get; private set; }
		public int Count => Leads.Count;

		public void Add(string phone, Lead lead) => Leads.Add(phone, lead);
		
		public bool Conains(string phone) => Leads.ContainsKey(phone);

		public string GetCollection()
		{
			return string.Empty;
		}
	}
}
