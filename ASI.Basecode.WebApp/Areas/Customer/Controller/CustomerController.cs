using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Areas.Customer.Controller
{
    [Authorize(Policy = "Customer")]
    [Area("Customer")]
    public class CustomerController : ControllerBase<CustomerController>
    {
        public CustomerController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {

        }

        public IActionResult Index()
        {
            ViewBag.UserEmail = User.Identity.Name;
            ViewBag.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  
            return View();
        }
    }
}
