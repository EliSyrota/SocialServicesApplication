using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SocialServicesApplication.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ConnectionController : Controller
	{
		private readonly Services _services;
		public ConnectionController(Services services)
		{
			_services = services;
		}

		[HttpGet]
		public ServicesStatus Get()
		{
			return new ServicesStatus()
			{
				AutoRunner = !string.IsNullOrWhiteSpace(_services.Settings.SourceUrl),
				TelegramServicesWorking = _services.TelegramService.IsConnected && _services.TelegramService.IsAuthorized,
				WebServiceConnection = _services.Settings.Running
			};
		}
	}
}