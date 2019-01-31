using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PtcApi.Security;
using PtcApi.Model;

namespace PtcApi.Controllers
{
    [Route("api/[controller]")]

    public class SecurityController : BaseApiController

    {
        private JwtSettings _settings;
        public SecurityController(JwtSettings settings)   
        {
            _settings = settings;
        } 

        [HttpPost("login")]
        public IActionResult Login([FromBody]AppUser user)
        {
            IActionResult ret  = null;
            AppUserAuth auth = new AppUserAuth();  //this is the payload that comes back from api to angular app
            SecurityManager mgr = new SecurityManager(_settings);

            auth = mgr.ValidateUser(user);

            if(auth.IsAuthenticated)
            {
                ret = StatusCode(StatusCodes.Status200OK, auth);   //auth is the returned payload to angular
            }
            else
            {
                ret = StatusCode(StatusCodes.Status404NotFound,"Invalid Username/Password.");
            }
            return ret;
        }
    }
}
