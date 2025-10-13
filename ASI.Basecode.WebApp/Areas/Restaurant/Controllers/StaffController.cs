using System;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Areas.Restaurant.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "RestaurantAdmin")]
    [Area("Restaurant")]
    public class StaffController : ControllerBase<StaffController>
    {
        public StaffController(
                                        IHttpContextAccessor httpContextAccessor,
                                        ILoggerFactory loggerFactory,
                                        IConfiguration configuration,
                                        IMapper mapper
                                    ) : base(httpContextAccessor, loggerFactory, configuration, mapper) { }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public Task<IActionResult> CreateStaffInfo()
        {
            // TODO: Implement Create logic
            return null;
        }

        [HttpPost]
        public Task<IActionResult> UpdateStaffInfo()
        {
            // TODO: Implement Update logic
            return null;
        }
    }
}
