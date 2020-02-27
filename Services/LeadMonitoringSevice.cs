using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace SocialServicesApplication
{
	public class LeadMonitoringSevice
	{
		private readonly Settings _settings;
		public LeadCollection LeadCachedCollection { get; }

		public LeadMonitoringSevice(Settings settings)
		{
			_settings = settings;
			LeadCachedCollection = new LeadCollection(new List<LeadRequest>());
		}

		public bool Cache(List<Lead> leadCollection)
		{
			foreach (var lead in leadCollection)
			{
				LeadCachedCollection.Leads[lead.Phone] = lead;
			}
			return true;
		}

		public string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}

		public List<LeadRequest> GetRequestedLeads()
		{
			string ret = string.Empty;
			var leads = new List<LeadRequest>();
			try
			{
				var token = CalculateMD5Hash($"LeadBolid-{DateTime.Now:yyyy-MM-dd}").ToLower();
				var sourceUrl = string.Concat(_settings.SourceUrl, $"?token={token}");
				var webRequest = System.Net.WebRequest.Create(sourceUrl) as HttpWebRequest;
				HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
				Stream resStream = resp.GetResponseStream();
				StreamReader reader = new StreamReader(resStream);
				ret = reader.ReadToEnd();

				var leadAutoRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<LeadAutoRequest>(ret);


				if (leadAutoRequest == null || leadAutoRequest.Items.Count <= 0)
				{
					return leads;
				}

				leads = leadAutoRequest.Items.Select(item => new LeadRequest() { Id = item.Lead, Phone = item.Phone })
					.ToList();
				return leads;
			}
			catch (Exception e)
			{
				return leads;
			}

		}

		public List<LeadRequest> GetNewRequests(List<LeadRequest> allRequestedLeads)
		{
			return allRequestedLeads.Where(lead => !LeadCachedCollection.Conains(lead.Phone)).ToList();

		}

		public bool SendResponce(LeadCollection leads)
		{
			if (leads.Count == 0)
			{
				return false;
			}
			var token = CalculateMD5Hash($"LeadBolid-{DateTime.Now:yyyy-MM-dd}").ToLower();
			var sourceUrl = string.Concat(_settings.SourceUrl, $"?token={token}");
			var client = new HttpClient { BaseAddress = new Uri(sourceUrl) };
			var request = new HttpRequestMessage(HttpMethod.Put, sourceUrl);
			request.Content = new FormUrlEncodedContent(getData(leads));
			var response = client.SendAsync(request);
			response.Wait();
			if (!response.IsCompletedSuccessfully) return false;
			var resultText = response.Result.Content.ReadAsStringAsync();
			resultText.Wait();
			var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(resultText.Result);
			if (result == null || string.IsNullOrWhiteSpace(result.Message))
			{
				return false;
			}

			return int.TryParse(string.Join("", result.Message.Where(char.IsDigit)), out var updatedCount) && updatedCount > 0;
		}

		private IEnumerable<KeyValuePair<string, string>> getData(LeadCollection leads)
		{
			var iterator = 0;
			foreach (var lead in leads.Leads)
			{
				yield return new KeyValuePair<string, string>($"socials[{iterator}][lead]", $"{lead.Value.Id}");
				yield return new KeyValuePair<string, string>($"socials[{iterator}][telegram]", $"{lead.Value.IsTelegram}");
				yield return new KeyValuePair<string, string>($"socials[{iterator}][viber]", $"{lead.Value.IsViber}");
				yield return new KeyValuePair<string, string>($"socials[{iterator}][whatsapp]", $"{lead.Value.IsWhatsApp}");
				iterator++;
			}
		}

		public class Result
		{
			public string Message { get; set; }
		}
	}
}