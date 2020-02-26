using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SocialServicesApplication.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GetSettingsController : Controller
	{
		private readonly TelegramSettings _telegramSettings;
		private readonly TelegramService _telegramService;
		private readonly Services _services;

		public GetSettingsController(Microsoft.Extensions.Options.IOptionsMonitor<TelegramSettings> telegramSettings,
			Services services)
		{
			_services = services;
			_telegramService = services.TelegramService;
			_telegramSettings = telegramSettings.CurrentValue;
		}

		[HttpGet]
		public IActionResult Get()
		{
			var result = new
			{
				ApiHash = _telegramService.TelegramSettings.ApiHash,
				ApiId = _telegramService.TelegramSettings.ApiId,
				NumberToAuthenticate = _telegramService.TelegramSettings.NumberToAuthenticate,
				CodeToAuthenticate = _telegramService.TelegramSettings.CodeToAuthenticate,
				Interval = _services.Settings.Interval,
				SourceUrl = _services.Settings.SourceUrl,
				Running = _services.Settings.Running
			};
			return Json(JsonConvert.SerializeObject(result));
		}
	}
}