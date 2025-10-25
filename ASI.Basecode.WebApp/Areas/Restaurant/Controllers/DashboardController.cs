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
                // Default period "monthly" 
                var stats = await _orderService.GetDashboardStatsAsync("monthly");
                var orderSummary = await _orderService.GetOrderSummaryAsync("monthly");
                var mostSellingItems = await _productService.GetMostSellingItemsAsync("monthly");
                var mostFavoriteItems = await _productService.GetMostFavoriteItemsAsync("monthly");
                var staffActivities = await _orderService.GetRecentStaffActivitiesAsync();
                var revenueData = await _orderService.GetRevenueDataAsync("monthly");
                var monthlyRevenue = await _orderService.GetMonthlyRevenueAsync("monthly");

                var dashboardData = new DashboardDataViewModel
                {
                    Stats = stats,
                    OrderSummary = orderSummary,
                    MostSellingItems = mostSellingItems ?? new List<MostSellingItemViewModel>(),
                    MostFavoriteItems = mostFavoriteItems ?? new List<MostFavoriteItemViewModel>(),
                    StaffActivities = staffActivities ?? new List<StaffActivityViewModel>(),
                    RevenueData = revenueData,
                    MonthlyRevenue = monthlyRevenue ?? new List<MonthlyRevenueViewModel>()
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
                    RevenueData = new RevenueViewModel(),
                    MonthlyRevenue = new List<MonthlyRevenueViewModel>()
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

        [HttpGet]
        public async Task<IActionResult> GetDashboardDataByPeriod(string period = "today")
        {
            try
            {
                // Apply period to relevant data
                var stats = await _orderService.GetDashboardStatsAsync(period);
                var orderSummary = await _orderService.GetOrderSummaryAsync(period);
                var mostSellingItems = await _productService.GetMostSellingItemsAsync(period); // Apply period here
                var mostFavoriteItems = await _productService.GetMostFavoriteItemsAsync(); // Keep as overall
                var staffActivities = await _orderService.GetRecentStaffActivitiesAsync();
                var revenueData = await _orderService.GetRevenueDataAsync(period);
                var monthlyRevenue = await _orderService.GetMonthlyRevenueAsync(period);

                var dashboardData = new DashboardDataViewModel
                {
                    Stats = stats,
                    OrderSummary = orderSummary,
                    MostSellingItems = mostSellingItems ?? new List<MostSellingItemViewModel>(),
                    MostFavoriteItems = mostFavoriteItems ?? new List<MostFavoriteItemViewModel>(),
                    StaffActivities = staffActivities ?? new List<StaffActivityViewModel>(),
                    RevenueData = revenueData,
                    MonthlyRevenue = monthlyRevenue ?? new List<MonthlyRevenueViewModel>()
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data for period: {Period}", period);
                return StatusCode(500, new { error = "Error loading dashboard data" });
            }
        }

        [HttpGet]
        public IActionResult DebugOrders()
        {
            try
            {
                _orderService.DebugOrderData();
                return Content("Order debug data printed to console. Check your server logs.");
            }
            catch (Exception ex)
            {
                return Content($"Debug error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDashboardData()
        {
            try
            {
                // Get data for all periods
                var todayData = await GetDashboardDataByPeriodInternal("today");
                var weeklyData = await GetDashboardDataByPeriodInternal("weekly");
                var monthlyData = await GetDashboardDataByPeriodInternal("monthly");

                return Ok(new
                {
                    today = todayData,
                    weekly = weeklyData,
                    monthly = monthlyData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all dashboard data");
                return StatusCode(500, new { error = "Error loading dashboard data" });
            }
        }

        private async Task<DashboardDataViewModel> GetDashboardDataByPeriodInternal(string period)
        {
            var stats = await _orderService.GetDashboardStatsAsync(period);
            var orderSummary = await _orderService.GetOrderSummaryAsync(period);
            var revenueData = await _orderService.GetRevenueDataAsync(period);
            var monthlyRevenue = await _orderService.GetMonthlyRevenueAsync(period);

            return new DashboardDataViewModel
            {
                Stats = stats,
                OrderSummary = orderSummary,
                RevenueData = revenueData,
                MonthlyRevenue = monthlyRevenue ?? new List<MonthlyRevenueViewModel>()
            };
        }
    }
}