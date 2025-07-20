using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SkillUpPlatform.Application.Interfaces
{
    public interface IUserContextService
    {
        int GetUserId();
        string GetUserEmail();
        string GetUserRole();
        string GetClientIpAddress();
        string GetUserAgent();
        ClaimsPrincipal GetUserClaimsPrincipal();
    }
}
