// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Data.Models
{
    using System;

    [Flags]
    public enum HttpMethods
    {
        GET = 1,
        HEAD = 2,
        POST = 4,
        PUT = 8,
        DELETE = 16,
        CONNECT = 32,
        OPTIONS = 64,
        TRACE = 128,
        PATCH = 252,
    }
}
