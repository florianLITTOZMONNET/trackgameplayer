using System.Net.Http.Json;
//using Core.Models.Riot;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ApiClients.Riot;

public class RiotMasteryClient : RiotApiClient
{
    public RiotMasteryClient(HttpClient httpClient, IConfiguration config)
        :base(httpClient, config){}
}