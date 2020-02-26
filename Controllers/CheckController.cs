using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TLSharp.Core;

namespace SocialServicesApplication.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class CheckController : Controller
	{
		private readonly ILogger<CheckController> _logger;
		private readonly TelegramService _telegramService;

		public CheckController(ILogger<CheckController> logger, Microsoft.Extensions.Options.IOptionsMonitor<TelegramSettings> telegramSettings,
			Services services)
		{
			_logger = logger;
			_telegramService = services.TelegramService;
			TelegramSettings = telegramSettings.CurrentValue;
		}

		public TelegramSettings TelegramSettings { get; private set; }

		[HttpPost]
		public IActionResult PostHandler([FromBody] IEnumerable<LeadRequest> leads)
		{
			if (!_telegramService.IsConnected || !_telegramService.IsAuthorized)
			{
				return new JsonResult(new TelegramSettingsMissingJsonResult(TelegramSettings));
			}
			//TODO: use other services


			var leadsCollection = new LeadCollection(leads.ToList());
			if (leadsCollection.Leads.Count == 0)
			{
				return new JsonResult(new LeadDataMissingJsonResult());
			}

			_telegramService.VerifyNumbers(leadsCollection);
			//TODO: use other services
			return new JsonResult(leadsCollection.GetLeadsList());
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok(null);
		}
	}
}
