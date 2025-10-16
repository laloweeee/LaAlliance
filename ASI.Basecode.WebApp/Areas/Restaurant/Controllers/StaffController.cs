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

        /// Displays the Update Staff Member form with existing staff data.
        [HttpGet]
        public IActionResult UpdateStaffMember(int id)
        {
            // To log the received ID
            _logger.LogInformation("UpdateStaffMember GET called with ID: {Id}", id);
            
            try
            {
                var staff = _staffService.GetStaffByID(id);

                if (staff == null)
                {
                    _logger.LogWarning("Staff member with ID {Id} not found", id);
                    TempData["Error"] = "Staff member not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Map RestaurantStaff entity to StaffViewModel
                var model = new StaffViewModel
                {
                    StaffID = staff.StaffID,
                    FirstName = staff.User?.UserProfile?.FirstName ?? string.Empty,
                    LastName = staff.User?.UserProfile?.LastName ?? string.Empty,
                    Email = staff.User?.Email ?? string.Empty,
                    ContactNumber = staff.User?.UserProfile?.ContactNumber ?? string.Empty,
                    Role = staff.Role.ToString(),
                    Status = staff.Status.ToString(),
                    // We don't retrieve password for security reasons
                };

                // We log what we're sending to the view
                _logger.LogInformation("Loading staff: ID={Id}, Name={FirstName} {LastName}, Email={Email}, Role={Role}", 
                    model.StaffID, model.FirstName, model.LastName, model.Email, model.Role);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff member with ID {Id}", id);
                TempData["Error"] = "An error occurred while retrieving staff information.";
                return RedirectToAction(nameof(Index));
            }
        }
        
        /// Processes the staff member update form submission.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStaffMember(StaffViewModel model)
        {
            // We log what we received
            _logger.LogInformation("UpdateStaffMember POST called with ID: {Id}, Name: {FirstName} {LastName}", 
                model.StaffID, model.FirstName, model.LastName);
            
            // We log ModelState errors
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("ModelState Error: {Error}", error.ErrorMessage);
                }
                return View(model);
            }

            try
            {
                _staffService.UpdateStaff(model);

                TempData["Success"] = "Staff member updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "ArgumentException updating staff: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff member with ID {Id}", model.StaffID);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the staff member.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult AddStaffMember()
        {
            return View();
        }

        /// <summary>
        /// Deletes a staff member
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStaff(int staffID)
        {
            _logger.LogInformation("DeleteStaff called for ID: {Id}", staffID);

            try
            {
                var staff = _staffService.GetStaffByID(staffID);
                
                if (staff == null)
                {
                    TempData["Error"] = "Staff member not found.";
                    return RedirectToAction(nameof(Index));
                }

                _staffService.DeleteStaff(staffID);

                TempData["Success"] = $"Staff member {staff.User?.UserProfile?.FirstName} {staff.User?.UserProfile?.LastName} has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff member with ID {Id}", staffID);
                TempData["Error"] = "An error occurred while deleting the staff member.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}