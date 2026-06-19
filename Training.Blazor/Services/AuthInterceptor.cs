using System.Net;

namespace Training.Blazor.Services
{
    public sealed class AuthInterceptor : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/Refresh");
                var refreshResponse = await base.SendAsync(refreshRequest, cancellationToken);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    var retryRequest = CloneRequest(request);
                    response = await base.SendAsync(retryRequest, cancellationToken);
                }
            }

            return response;
        }

        private static HttpRequestMessage CloneRequest(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri)
            {
                Content = req.Content,
                Version = req.Version
            };
            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}
