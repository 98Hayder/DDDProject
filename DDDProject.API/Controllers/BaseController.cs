using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DDDProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected virtual string GetClaim(string claimName)
        {
            var claims = (User.Identity as ClaimsIdentity)?.Claims;
            var claim = claims?.FirstOrDefault(c =>
                string.Equals(c.Type, claimName, StringComparison.CurrentCultureIgnoreCase) &&
                !string.Equals(c.Type, "null", StringComparison.CurrentCultureIgnoreCase));
            var rr = claim?.Value!.Replace("\"", "");

            return rr ?? "";
        }

        protected int Id => int.TryParse(GetClaim("Id"), out var id) ? id : 0;

        protected string Role => GetClaim("Role");


    }
}
