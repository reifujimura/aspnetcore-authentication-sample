using System;

namespace AuthenticationSample
{
    static class AppConfig
    {
        public static readonly string SiteUrl = "http://localhost:5000";
        public static readonly byte[] SecretKey = Guid.NewGuid().ToByteArray();
    }
}