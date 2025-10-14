using System;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;                  
using ASI.Basecode.Services.Interfaces; 

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "RestaurantAdmin")]
    [Area("Restaurant")]
    public class StaffController : ControllerBase<StaffController>
    {
        private readonly IStaffService _staffService; 
        public StaffController(
                                        IHttpContextAccessor httpContextAccessor,
                                        ILoggerFactory loggerFactory,
                                        IConfiguration configuration,
                                        IMapper mapper,
                                        IStaffService staffService  
                                    ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
             _staffService = staffService ?? throw new ArgumentNullException(nameof(staffService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<StaffList> vm = await _staffService.ListAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStaffInfo(StaffViewModel model) 
        {
            if (!ModelState.IsValid)
                return View("AddStaffMember", model);

            try
            {
                _staffService.AddStaff(model);

                TempData["Success"] = "Staff member added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("AddStaffMember", model);
            }
        }

        [HttpPost]
        public IActionResult UpdateStaffInfo(StaffViewModel model)
        {
            // implement when ready using _staffService.UpdateStaff(model)
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public IActionResult AddStaffMember()
        {
            return View();
        }
    }
}
