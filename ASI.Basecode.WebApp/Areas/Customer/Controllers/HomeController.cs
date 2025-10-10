using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IProductService productService,
                              ICategoryService categoryService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var model = new Models.HomeViewModel()
            {
                Categories = _categoryService.GetAllCategories().ToList().AsQueryable(),
                Products = _productService.GetActiveProducts().ToList().AsQueryable()
            };

            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;

            return View(model);
        }
    }
}