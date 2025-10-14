using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : ControllerBase<CartController>
    {
        private new readonly ICartService _cartService;
        public CartController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              ICartService cartService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cartViewModel = _mapper.Map<CartViewModel>(_cartService.GetOrCreateCart(UserId));
            return View(cartViewModel);
        }
    }
}