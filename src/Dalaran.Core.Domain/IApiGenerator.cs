// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Domain
{
    public interface IApiGenerator
    {
        Api GenerateApi(ApiConfiguration configuration);
    }
}
