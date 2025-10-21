using Microsoft.AspNetCore.Mvc;
using ASI.Basecode.WebApp.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using ASI.Basecode.WebApp.Mvc;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderTrackingController : ControllerBase<OrderTrackingController>
    {
        public OrderTrackingController(IHttpContextAccessor httpContextAccessor,
                                       ILoggerFactory loggerFactory,
                                       IConfiguration configuration,
                                       IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
        }

        public IActionResult Index()
        {
            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleMapsApiKey;
            // Prefer coordinates saved in session during checkout (DeliveryLat/DeliveryLng)
            // These are set by CheckoutController.PlaceOrder so the tracking map uses the exact
            // location the customer chose at checkout. If not present, fall back to sample coords.
            string deliveryLatStr = null;
            string deliveryLngStr = null;
            string deliveryAddress = null;
            try
            {
                deliveryLatStr = _session?.GetString("DeliveryLat");
                deliveryLngStr = _session?.GetString("DeliveryLng");
                deliveryAddress = _session?.GetString("DeliveryAddress");
            }
            catch
            {
                // session might be unavailable in some contexts; ignore and use defaults
            }

            double deliveryLat = 10.298168; // sample default
            double deliveryLng = 123.952467; // sample default
            if (!string.IsNullOrEmpty(deliveryLatStr) && !string.IsNullOrEmpty(deliveryLngStr))
            {
                if (double.TryParse(deliveryLatStr, out var parsedLat)) deliveryLat = parsedLat;
                if (double.TryParse(deliveryLngStr, out var parsedLng)) deliveryLng = parsedLng;
            }

            // Create sample data for testing
            var viewModel = new OrderTrackingViewModel
            {
                CurrentOrder = new CurrentOrderViewModel
                {
                    OrderNumber = "FF2024001",
                    Status = "Out for Delivery",
                    ProgressPercentage = 75,
                    ETA = "18 mins",
                    OrderSteps = new List<OrderStepViewModel>
                    {
                        new OrderStepViewModel {
                            StepName = "Order Confirmed",
                            IsCompleted = true,
                            IconClass = "fas fa-check"
                        },
                        new OrderStepViewModel {
                            StepName = "Preparing Your Order",
                            IsCompleted = true,
                            IconClass = "fas fa-utensils"
                        },
                        new OrderStepViewModel {
                            StepName = "Out for Delivery",
                            IsCompleted = true,
                            IsCurrent = true,
                            IconClass = "fas fa-motorcycle"
                        },
                        new OrderStepViewModel {
                            StepName = "Delivered",
                            IsCompleted = false,
                            IconClass = "fas fa-home"
                        }
                    },
                    // Sample restaurant location (replace with actual data)
                    RestaurantLocation = new LocationViewModel
                    {
                        Latitude = 10.354070,
                        Longitude =  123.914220,
                        Address = "Fast Food Restaurant, Tanjay City",
                        Title = "Restaurant"
                    },

                    // Delivery location: use checkout-selected coordinates when available
                    DeliveryLocation = new LocationViewModel
                    {
                        Latitude = deliveryLat,
                        Longitude = deliveryLng,
                        Address = deliveryAddress ?? "Customer Address, Tanjay City",
                        Title = "Delivery Address"
                    },
                    // Sample driver location (replace with real-time data)
                    DriverLocation = new LocationViewModel
                    {
                        Latitude = 10.354070,
                        Longitude =  123.914220,
                        Address = "Driver Current Location",
                        Title = "Driver"
                    }
                }
            };

            return View(viewModel);
        }
    }
}