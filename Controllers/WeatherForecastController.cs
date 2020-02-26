using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SocialServicesApplication.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class LeadsHistoryController : Controller
	{

		private readonly Services _services;
		private readonly ILogger<LeadsHistoryController> _logger;

		public LeadsHistoryController(ILogger<LeadsHistoryController> logger, Services services)
		{
			_logger = logger;
			_services = services;
		}

		[HttpGet]
		public IEnumerable<Lead> Get()
		{
			if (_services?.LeadMonitoringSevice?.LeadCachedCollection == null || _services.LeadMonitoringSevice.LeadCachedCollection.Count <= 0) return new List<Lead>();
			var cachedLeads = _services.LeadMonitoringSevice.LeadCachedCollection.Leads.Values;
			return cachedLeads.ToArray();
		}
	}
}
