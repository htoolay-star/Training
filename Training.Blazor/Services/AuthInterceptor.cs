using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Training.Blazor.Services
{
    public sealed class AuthInterceptor : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            byte[]? contentBytes = null;
            if (request.Content is not null)
            {
                contentBytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
                var buffered = new ByteArrayContent(contentBytes);
                foreach (var header in request.Content.Headers)
                    buffered.Headers.TryAddWithoutValidation(header.Key, header.Value);
                request.Content = buffered;
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/Refresh");
                refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
                var refreshResponse = await base.SendAsync(refreshRequest, cancellationToken).ConfigureAwait(false);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    var retryRequest = CloneRequest(request, contentBytes);
                    response = await base.SendAsync(retryRequest, cancellationToken).ConfigureAwait(false);
                }
            }

            return response;
        }

        private static HttpRequestMessage CloneRequest(HttpRequestMessage req, byte[]? contentBytes)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri)
            {
                Version = req.Version
            };

            if (contentBytes is not null && req.Content is not null)
            {
                clone.Content = new ByteArrayContent(contentBytes);
                foreach (var header in req.Content.Headers)
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}
