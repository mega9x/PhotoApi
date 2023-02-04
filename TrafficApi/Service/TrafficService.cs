using Crawler;
using Models;
using Models.Config;
using Models.Response;
using Models.TrafficBackend.Config;
using System.Linq;
using Microsoft.AspNetCore.Routing.Constraints;
using Timer = System.Timers.Timer;
using System.Timers;

namespace TrafficApi.Service
{
    public class TrafficService
    {
        private bool _first = true;
        private readonly LlinksCrawler _crawler;
        private readonly General _general;
        private readonly Timer _timer;
        private IList<GroupTraffic> _groupTraffics = new List<GroupTraffic>();
        public ConfigService Config { get; }
        
        public TrafficService(ConfigService config)
        {
            _timer = new Timer(10000);
            _timer.Elapsed += async (object StringRouteConstraint, ElapsedEventArgs e) =>
            {
                Console.WriteLine("reloading...");
                _groupTraffics = await _crawler.GetAllTraffics();
            };
            _timer.Start();
            Config = config;
            _general = Config.Config.Value.General;
            _crawler = new LlinksCrawler(_general.Account, _general.Password);
        }
        public async Task<IEnumerable<SingleTraffic>?> GetTraffics(string id)
        {
            if (_first) _groupTraffics = await _crawler.GetAllTraffics();
            _first = false;
            var singleGroup = _groupTraffics.FirstOrDefault(predicate: (x) => x!.Id == id, defaultValue: null);
            return singleGroup is null ? new List<SingleTraffic>() : singleGroup.TrafficGroup;
        }
        public async Task<IEnumerable<GroupTraffic>> GetAllTraffics()
        {
            return _groupTraffics;
        }

    }
}
