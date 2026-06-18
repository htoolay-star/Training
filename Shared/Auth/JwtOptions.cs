using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Auth
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; init; } = "SmsGateway";
        public string Audience { get; init; } = "SmsGateway";

        public string SigningKey { get; init; } = string.Empty;

        public int AccessTokenMinutes { get; init; } = 60;
        public int RefreshTokenDays { get; init; } = 7;
    }
}
