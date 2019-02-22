// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Console
{
    internal static class Consts
    {
        public static class IdentityServer
        {
            public const string Url = "http://localhost:5005";

            public static class Api
            {
                public const string Url = "http://localhost:5050";
                public const string Id = "dalaran_api";
            }

            public static class Client
            {
                public const string Id = "dalaran_console";
                public const string RedirectUri = "http://127.0.0.1";
                public static readonly string Scope = $"openid profile email offline_access {Api.Id} ";
            }
        }
    }
}
