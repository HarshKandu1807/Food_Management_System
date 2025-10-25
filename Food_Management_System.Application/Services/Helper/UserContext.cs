using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.Helper
{
    public class UserContext:IUserContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserContext(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
        }
        public int? UserId { get
            {
                var user = httpContextAccessor.HttpContext?.User;
                if (user == null) return null;
                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim == null) return null;
                return int.TryParse(idClaim.Value, out var id) ? id : null;
            }
        }
    }
}
