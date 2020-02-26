using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SocialServicesApplication.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class SettingsController : Controller
	{
		private readonly Services _services;

		public SettingsController(Services services)
		{
			_services = services;
		}

		[HttpPost]
		public IActionResult PostHandler([FromBody] Settings settings)
		{
			_services.Settings = settings;
			_services.Reset();
			return new JsonResult(new SettingsSuccessfulJsonResult());
		}
	}
}