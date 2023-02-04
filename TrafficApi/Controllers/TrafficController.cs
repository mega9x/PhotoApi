using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Response;
using TrafficApi.Service;

namespace TrafficApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficController : ControllerBase
    {
        private readonly TrafficService _trafficService;
        public TrafficController(TrafficService trafficService)
        {
            _trafficService = trafficService;
        }
        [HttpGet("gettraffic")]
        public async Task<IEnumerable<SingleTraffic>?> GetTraffics(string gid) => await _trafficService.GetTraffics(gid);
        [HttpGet("getalltraffic")]
        public async Task<IEnumerable<GroupTraffic>> GetAllTraffics() => await _trafficService.GetAllTraffics();
    }
}
