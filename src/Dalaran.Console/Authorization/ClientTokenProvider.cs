// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Authorization
{
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public class ClientTokenProvider
    {
        private readonly TokenEndpointProvider tokenEndpointProvider;
        private readonly string clientId;
        private readonly string clientSecret;

        public ClientTokenProvider(TokenEndpointProvider tokenEndpointProvider, string clientId, string clientSecret)
        {
            this.tokenEndpointProvider = tokenEndpointProvider;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        public async Task<string> GetClientToken(string scope)
        {
            string tokenEndpoint = await this.tokenEndpointProvider.GetEndpointAsync().ConfigureAwait(false);

            using (var tokenClient = new TokenClient(tokenEndpoint, this.clientId, this.clientSecret))
            {
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope).ConfigureAwait(false);

                return tokenResponse.AccessToken;
            }
        }
    }
}
