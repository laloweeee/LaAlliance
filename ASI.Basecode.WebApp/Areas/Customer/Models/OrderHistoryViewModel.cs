using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class OrderHistoryViewModel
    {
        public string ActiveTab { get; set; } = "ongoing";
        public List<OrderViewModel> OngoingOrders { get; set; }
        public List<OrderViewModel> OrderHistory { get; set; }
    }
}