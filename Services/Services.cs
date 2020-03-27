using System;
using System.Linq;
using System.Security.Policy;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TLSharp.Core;
using Timer = System.Timers.Timer;

namespace SocialServicesApplication
{

    public class Services
    {
        public Timer Timer { get; private set; }

        public Settings Settings { get; set; }

        public TelegramService TelegramService { get; private set; }

        public LeadMonitoringSevice LeadMonitoringSevice { get; private set; }


        private static bool _isRunning;

        public async void Run()
        {
            // Get new batch to check
            _isRunning = true;
            var newLeads = LeadMonitoringSevice.GetRequestedLeads();
            if (newLeads.Count == 0)
            {
                return;
            }
            var allLeads = new LeadCollection(newLeads);
            if (TelegramService.IsConnected && TelegramService.IsAuthorized)
            {
                TelegramService.VerifyNumbers(allLeads, LeadMonitoringSevice.GetNewRequests(newLeads));
                foreach (var lead in allLeads.Leads)
                {
                    if (LeadMonitoringSevice.LeadCachedCollection.TryGetLead(lead.Key, out Lead cachedLead))
                    {
                        lead.Value.IsTelegram = cachedLead.IsTelegram;
                        lead.Value.IsViber = cachedLead.IsViber;
                        lead.Value.IsWhatsApp = cachedLead.IsWhatsApp;
                        lead.Value.TelegramUser = cachedLead.TelegramUser;
                    }
                }

                if (LeadMonitoringSevice.SendResponce(allLeads))
                {
                    // TODO: logger
                    LeadMonitoringSevice.Cache(allLeads.Leads.Values.ToList());
                }
            }

            _isRunning = false;
        }

        public Services(Settings settings, TelegramSettings telegramSettings)
        {
            LeadMonitoringSevice = new LeadMonitoringSevice(settings);
            TelegramService = new TelegramService(telegramSettings, LeadMonitoringSevice);
            Settings = settings;
            SerilogSetup();
            Reset();
        }

        private void SerilogSetup()
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                });

            var logger = loggerConfig.CreateLogger();
            Log.Logger = logger;
        }

        public void AddService(IServiceSettings settings, IClient client)
        {

        }

        public void Reset()
        {
            if (Settings.Interval == 0)
            {
                return;
            }
            Timer = new Timer(Settings.Interval);
            Timer.Elapsed += (s, e) => { if (!_isRunning && Settings.Running) Run(); };
            if (Settings.Running)
            {
                Timer.Start();
            }
            else
            {
                Timer.Stop();
            }

            _isRunning = false;
        }
    }
}
