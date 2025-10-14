using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "RestaurantStaff")]
    [Area("Restaurant")]
    public class OrdersController : ControllerBase<OrdersController>
    {
        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper) { }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.UserEmail = User.Identity?.Name;
            ViewBag.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View();
        }
    }
}
