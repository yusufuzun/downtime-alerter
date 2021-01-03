using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alerter.WebApp.AlertHttpClient
{
    public class StatusCheckClient : IStatusCheckClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        public StatusCheckClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CheckStatus2XX(string url)
        {
            var response = await httpClientFactory.CreateClient("StatusCheckClient").GetAsync(url);

            return response != null && (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;
        }
    }
}
