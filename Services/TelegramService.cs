using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TeleSharp.TL;
using TLSharp.Core;
using TLSharp.Core.Exceptions;

namespace SocialServicesApplication
{
	public class TelegramService
	{
		private readonly LeadMonitoringSevice _services;
		public TelegramSettings TelegramSettings { get; private set; }


		public TelegramClient TelegramClient { get; private set; }

		public TelegramService(TelegramSettings telegramSettings, LeadMonitoringSevice services)
		{
			_services = services;
			Reset(telegramSettings);
		}

		public void Reset(TelegramSettings telegramSettings)
		{
			TelegramSettings = telegramSettings;
			try
			{
				if (string.IsNullOrWhiteSpace(TelegramSettings.ApiId) ||
					 string.IsNullOrWhiteSpace(TelegramSettings.ApiHash) ||
					 string.IsNullOrWhiteSpace(TelegramSettings.NumberToAuthenticate) ||
					 string.IsNullOrWhiteSpace(TelegramSettings.CodeToAuthenticate))
				{
					return;
				}
				TelegramClient = new TelegramClient(int.Parse(TelegramSettings.ApiId), TelegramSettings.ApiHash);
				//AuthUser().Wait();
			}
			catch (MissingApiConfigurationException ex)
			{
				/*
				throw new Exception(
					$"Please add your API settings to the `app.config` file. (More info: {MissingApiConfigurationException.InfoUrl})",
					ex);
					*/
			}
		}

		public bool IsConnected
		{
			get
			{
				try
				{
					return TelegramClient != null && TelegramClient.IsConnected;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					return false;
				}
			}
		}

		public bool IsAuthorized { get; private set; }

		public bool VerifyNumbers(LeadCollection allLeadCollection, List<LeadRequest> firstTimeRequestedLeads)
		{
			if (!IsConnected || !IsAuthorized)
			{
				throw new InvalidOperationException("Service isn't running. Start service before executing some commands");
				return false;
			}

			if (firstTimeRequestedLeads.Count == 0) return true;

			try
			{
				var contacts = firstTimeRequestedLeads.Select(lead => new LeadToContactAdapter(lead.Id.ToString(), lead.Phone))
					.ToList();
				var task = TelegramClient.ImportContactsAsync(contacts);
				task.Wait();
				var imported = task.Result.Users;
				var toDeleteUsers = new List<TLAbsInputUser>();
				var importedUsers = imported.Select(user => user as TLUser)
					.Where(user => user != null && !string.IsNullOrWhiteSpace(user.Username)).ToDictionary(user => user.Phone);
				foreach (var lead in allLeadCollection.Leads)
				{
					if (importedUsers.TryGetValue(lead.Key, out TLUser importedUser))
					{
						toDeleteUsers.Add(new TLInputUser() { AccessHash = importedUser.AccessHash ?? 0, UserId = importedUser.Id });
						lead.Value.IsTelegram = true;
						lead.Value.TelegramUser = importedUser.Username;
					}
				}

				if (toDeleteUsers.Count > 0)
				{
					TelegramClient.DeleteContactsAsync(toDeleteUsers).Wait();
				}

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return false;
			}
			return true;
		}

		public bool VerifyNumbers(LeadCollection leadCollection)
		{
			if (!IsConnected || !IsAuthorized)
			{
				//throw new InvalidOperationException("Service isn't running. Start service before executing some commands");
				return false;
			}

			if (leadCollection.Count == 0) return true;

			try
			{
				var contacts = leadCollection.Leads.Select(lead => new LeadToContactAdapter(lead.Value.Id.ToString(), lead.Value.Phone))
					.ToList();
				var task = TelegramClient.ImportContactsAsync(contacts);
				task.Wait();
				var imported = task.Result.Users;
				var toDeleteUsers = new List<TLAbsInputUser>();
				foreach (var user in imported.Select(user => user as TLUser).Where(user => user != null && !string.IsNullOrWhiteSpace(user.Username)))
				{
					if (leadCollection.TryGetLead(user.Phone, out var lead))
					{
						lead.IsTelegram = true;
						lead.TelegramUser = user.Username;
					}
					toDeleteUsers.Add(new TLInputUser() { AccessHash = user.AccessHash ?? 0, UserId = user.Id });
				}

				if (toDeleteUsers.Count > 0)
				{
					TelegramClient.DeleteContactsAsync(toDeleteUsers).Wait();
				}

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return false;
			}
			return true;
		}

		public async Task AuthUser()
		{
			if (TelegramClient == null)
			{
				throw new Exception("CodeToAuthenticate is empty in the app.config file, fill it with the code you just got now by SMS/Telegram");
			}
			await TelegramClient.ConnectAsync();

			var hash = await TelegramClient.SendCodeRequestAsync(TelegramSettings.NumberToAuthenticate);
			var code = TelegramSettings.CodeToAuthenticate; // you can change code in debugger too

			if (String.IsNullOrWhiteSpace(code))
			{
				throw new Exception("CodeToAuthenticate is empty in the app.config file, fill it with the code you just got now by SMS/Telegram");
			}

			TLUser user = null;
			try
			{
				user = await TelegramClient.MakeAuthAsync(TelegramSettings.NumberToAuthenticate, hash, code);
				if (user != null)
				{
					IsAuthorized = true;
				}
			}
			catch (CloudPasswordNeededException ex)
			{
				var passwordSetting = await TelegramClient.GetPasswordSetting();
				var password = TelegramSettings.PasswordToAuthenticate;

				user = await TelegramClient.MakeAuthWithPasswordAsync(passwordSetting, password);
			}
			catch (InvalidPhoneCodeException ex)
			{
				throw;
			}
		}

	}

	internal class LeadToContactAdapter : TLInputPhoneContact
	{
		public LeadToContactAdapter(string firstName, string phone)
		{
			FirstName = firstName;
			LastName = "";
			Phone = phone;
		}
	}
}