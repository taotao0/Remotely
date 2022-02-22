using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URemote.Server.Auth
{
    public class TwoFactorRequiredRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "TwoFactorRequired";
    }
}
