using ASI.Basecode.WebApp.Mvc;              
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class UserController : ControllerBase<UserController>
    {
        public UserController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper) { }

        public IActionResult Profile()
        {
            return View(); 
        }

        public IActionResult Settings()
        {
            return View(); 
        }
    }
}