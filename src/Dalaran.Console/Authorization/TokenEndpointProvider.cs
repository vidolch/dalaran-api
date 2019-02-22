// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console.Authorization
{
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public class TokenEndpointProvider
    {
        private string authority;
        private string endpoint;

        public TokenEndpointProvider(string authority)
        {
            this.authority = authority;
        }

        public async Task<string> GetEndpointAsync()
        {
            if (!string.IsNullOrWhiteSpace(this.endpoint))
            {
                return this.endpoint;
            }

            using (var discoveryClient = new DiscoveryClient(this.authority) { Policy = new DiscoveryPolicy { ValidateIssuerName = false } })
            {
                var discoveryResponse = await discoveryClient.GetAsync().ConfigureAwait(false);
                this.endpoint = discoveryResponse.TokenEndpoint;

                return this.endpoint;
            }
        }
    }
}
