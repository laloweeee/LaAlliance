using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class DashboardController : ControllerBase<DashboardController>
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public DashboardController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IOrderService orderService,
            IProductService productService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _orderService = orderService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserEmail = User.Identity?.Name;
            ViewBag.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var stats = await _orderService.GetDashboardStatsAsync();
                var orderSummary = await _orderService.GetOrderSummaryAsync();
                var mostSellingItems = await _productService.GetMostSellingItemsAsync();
                var mostFavoriteItems = await _productService.GetMostFavoriteItemsAsync();
                var staffActivities = await _orderService.GetRecentStaffActivitiesAsync();
                var revenueData = await _orderService.GetRevenueDataAsync(); // Add this
                var monthlyRevenue = await _orderService.GetMonthlyRevenueAsync(); // Add this

                var dashboardData = new DashboardDataViewModel
                {
                    Stats = stats,
                    OrderSummary = orderSummary,
                    MostSellingItems = mostSellingItems ?? new List<MostSellingItemViewModel>(),
                    MostFavoriteItems = mostFavoriteItems ?? new List<MostFavoriteItemViewModel>(),
                    StaffActivities = staffActivities ?? new List<StaffActivityViewModel>(),
                    RevenueData = revenueData, // Add this
                    MonthlyRevenue = monthlyRevenue ?? new List<MonthlyRevenueViewModel>() // Add this
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                // Log the error and return empty dashboard
                _logger.LogError(ex, "Error loading dashboard data");
                
                var emptyDashboardData = new DashboardDataViewModel
                {
                    Stats = new DashboardStatsViewModel(),
                    OrderSummary = new OrderSummaryViewModel(),
                    MostSellingItems = new List<MostSellingItemViewModel>(),
                    MostFavoriteItems = new List<MostFavoriteItemViewModel>(),
                    StaffActivities = new List<StaffActivityViewModel>(),
                    RevenueData = new RevenueViewModel(), // Add this
                    MonthlyRevenue = new List<MonthlyRevenueViewModel>() // Add this
                };

                return View(emptyDashboardData);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var stats = await _orderService.GetDashboardStatsAsync();
            var orderSummary = await _orderService.GetOrderSummaryAsync();
            var mostSellingItems = await _productService.GetMostSellingItemsAsync();
            var mostFavoriteItems = await _productService.GetMostFavoriteItemsAsync();
            var staffActivities = await _orderService.GetRecentStaffActivitiesAsync(); // Add this

            return Ok(new DashboardDataViewModel
            {
                Stats = stats,
                OrderSummary = orderSummary,
                MostSellingItems = mostSellingItems,
                MostFavoriteItems = mostFavoriteItems,
                StaffActivities = staffActivities // Add this
            });
        }
    }
}