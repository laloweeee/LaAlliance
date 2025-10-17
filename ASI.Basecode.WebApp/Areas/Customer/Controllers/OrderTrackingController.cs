using Microsoft.AspNetCore.Mvc;
using ASI.Basecode.WebApp.Models;
using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderTrackingController : Controller
    {
        public IActionResult Index()
        {
            // Add your Google Maps API key to appsettings.json
            ViewBag.GoogleMapsApiKey = "AIzaSyCJ5dBVFR0g-nYJwXv04NYuqoZLu37WqRw";
            
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
                        Latitude = 9.3148,
                        Longitude = 123.3029,
                        Address = "Fast Food Restaurant, Tanjay City",
                        Title = "Restaurant"
                    },
                    // Sample delivery location (replace with actual customer address)
                    DeliveryLocation = new LocationViewModel
                    {
                        Latitude = 9.3200,
                        Longitude = 123.3100,
                        Address = "Customer Address, Tanjay City",
                        Title = "Delivery Address"
                    },
                    // Sample driver location (replace with real-time data)
                    DriverLocation = new LocationViewModel
                    {
                        Latitude = 9.3170,
                        Longitude = 123.3060,
                        Address = "Driver Current Location",
                        Title = "Driver"
                    }
                }
            };

            return View(viewModel);
        }
    }
}