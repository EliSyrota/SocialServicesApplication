using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TLSharp.Core.Exceptions;

namespace SocialServicesApplication.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController : Controller
	{
		private readonly TelegramSettings _telegramSettings;
		private readonly TelegramService _telegramService;

		public AuthController(Microsoft.Extensions.Options.IOptionsMonitor<TelegramSettings> telegramSettings,
			Services services)
		{
			_telegramService = services.TelegramService;
			_telegramSettings = telegramSettings.CurrentValue;
		}

		[HttpPost]
		public IActionResult SetTelegramSettings([FromBody] TelegramSetupSettings telegramSetupSettings)
		{
			if (!string.IsNullOrWhiteSpace(telegramSetupSettings.CodeToAuthenticate))
			{
				if (string.IsNullOrWhiteSpace(telegramSetupSettings.ApiHash) ||
					 string.IsNullOrWhiteSpace(telegramSetupSettings.ApiId) ||
					 string.IsNullOrWhiteSpace(telegramSetupSettings.NumberToAuthenticate))
				{
					return new JsonResult(new TelegramSettingsMissingJsonResult(telegramSetupSettings));
				}
				// trying to auth
				_telegramSettings.ApiId = telegramSetupSettings.ApiId;
				_telegramSettings.ApiHash = telegramSetupSettings.ApiHash;
				_telegramSettings.NumberToAuthenticate = telegramSetupSettings.NumberToAuthenticate;
				_telegramSettings.CodeToAuthenticate = telegramSetupSettings.CodeToAuthenticate;
				_telegramService.Reset(_telegramSettings);

				try
				{
					_telegramService.AuthUser().Wait();
                    var info = new AuthSuccessfulJsonResult();
					Log.Logger.Information(info.MessageText);
					return new JsonResult(info);
				}
				catch (Exception e)
				{
					if (e.InnerException != null && !string.IsNullOrWhiteSpace(e.InnerException.Message) &&
						 (e.InnerException.Message.Contains(
							 "The numeric code used to authenticate does not match the numeric code sent by SMS/Telegram") || e.InnerException.Message.Contains("AUTH_RESTART")))
					{
						return new JsonResult(new TelegramTwoWayAuthPerformingResult());
					}
					Console.WriteLine(e);
					return new JsonResult(new TelegramSettingsMissingJsonResult(telegramSetupSettings));
				}
			}

			if (string.IsNullOrWhiteSpace(telegramSetupSettings.ApiHash) || string.IsNullOrWhiteSpace(telegramSetupSettings.ApiId) || string.IsNullOrWhiteSpace(telegramSetupSettings.NumberToAuthenticate))
			{
				return new JsonResult(new TelegramSettingsMissingJsonResult(telegramSetupSettings));
			}

			_telegramSettings.ApiId = telegramSetupSettings.ApiId;
			_telegramSettings.ApiHash = telegramSetupSettings.ApiHash;
			_telegramSettings.NumberToAuthenticate = telegramSetupSettings.NumberToAuthenticate;
			_telegramSettings.CodeToAuthenticate = string.Empty;
			try
			{
				_telegramService.Reset(_telegramSettings);
				_telegramService.AuthUser().Wait();
				return new JsonResult(new TelegramTwoWayAuthPerformingResult());
			}
			catch (InvalidPhoneCodeException ex)
			{
				return new JsonResult(new TelegramTwoWayAuthPerformingResult());
			}
			catch (Exception e)
			{
				return new JsonResult(new TelegramTwoWayAuthPerformingResult());
			}
			return Ok(null);
		}
	}
}