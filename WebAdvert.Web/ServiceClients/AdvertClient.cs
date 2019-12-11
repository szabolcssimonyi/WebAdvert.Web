using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebAdvert.Web.Interfaces;
using WebAdvert.Web.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertClient : IAdvertApiClient
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client;
        private readonly IMapper mapper;

        public AdvertClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            this.configuration = configuration;
            this.client = client;
            this.mapper = mapper;
            var createUrl = configuration.GetSection("AdvertApi").GetValue<string>("CreateUrl");
            this.client.BaseAddress = new Uri(createUrl);
            this.client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        }

        public async Task<CreateAdvertResponse> Create(Models.AdvertModel model)
        {
            var advertApiModel = mapper.Map<AdvertApi.Models.AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertApiModel);
            var response = await client.PostAsync(client.BaseAddress, new StringContent(jsonModel));
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var createAdvertResponse = JsonConvert.DeserializeObject<AdvertApi.Models.CreateAdvertResponse>(jsonResponse);
            var advertResponse = mapper.Map<Models.CreateAdvertResponse>(createAdvertResponse);
            return advertResponse;
        }
    }
}
