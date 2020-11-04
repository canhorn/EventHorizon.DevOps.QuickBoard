using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventHorizon.DevOps.QuickBoard.WorkItems.Services
{
    public abstract class AzureDevOpsBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public AzureDevOpsBase(
            IHttpClientFactory clientFactory
        )
        {
            _clientFactory = clientFactory;
        }

        protected async Task<ServiceResult<T>> Get<T>(
            string url,
            string accessToken
        )
        {
            // Exchange the auth code for an access token and refresh token
            var requestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                url
            );
            requestMessage.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"
                )
            );
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );

            var responseMessage = await _clientFactory
                .CreateClient()
                .SendAsync(
                    requestMessage
                );

            if (responseMessage.IsSuccessStatusCode)
            {
                var body = await responseMessage.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<T>(
                    body
                );
                return new ServiceResult<T>(
                    true,
                    result
                );
            }

            return new ServiceResult<T>(
                "not_success_status_code"
            );
        }

        protected async Task<ServiceResult<T>> Post<T>(
            string url,
            string accessToken,
            object data
        )
        {
            // Exchange the auth code for an access token and refresh token
            var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                url
            );
            requestMessage.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"
                )
            );
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );
            var content = JsonConvert.SerializeObject(
                data
            );
            requestMessage.Content = new StringContent(
                content,
                Encoding.UTF8,
                "application/json-patch+json"
            );

            var responseMessage = await _clientFactory
                .CreateClient()
                .SendAsync(
                    requestMessage
                );
            var body = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                body = await responseMessage.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<T>(
                    body
                );
                return new ServiceResult<T>(
                    true,
                    result
                );
            }

            return new ServiceResult<T>(
                "not_success_status_code"
            );
        }

        public class ServiceResult<T>
        {
            public bool Successful { get; }
            public T Result { get; }
            public string ErrorCode { get; }

            public ServiceResult(
                bool successful,
                T result
            )
            {
                Successful = successful;
                Result = result;
                ErrorCode = string.Empty;
            }

            public ServiceResult(
                string errorCode
            )
            {
                Successful = false;
                Result = default;
                ErrorCode = errorCode;
            }
        }
    }
}
