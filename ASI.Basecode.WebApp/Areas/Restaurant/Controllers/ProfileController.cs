using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using ASI.Basecode.WebApp.Areas.Restaurant.Models;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class ProfileController : ControllerBase<ProfileController>
    {
        public ProfileController(
                                        IHttpContextAccessor httpContextAccessor,
                                        ILoggerFactory loggerFactory,
                                        IConfiguration configuration,
                                        IMapper mapper
                                    ) : base(httpContextAccessor, loggerFactory, configuration, mapper) { }
        public IActionResult Index()
        {
            return View();
        }
    }
}
