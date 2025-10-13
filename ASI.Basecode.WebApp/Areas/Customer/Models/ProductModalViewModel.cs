using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class ProductModalViewModel
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsActive { get; set; }
        public List<ProductOptionGroupViewModel> CustomizationGroups { get; set; } = new List<ProductOptionGroupViewModel>();
    }

    public class ProductOptionGroupViewModel
    {
        public int ProductOptionGroupID { get; set; }
        public int ProductID { get; set; }
        public string OptionGroupName { get; set; }
        public bool IsRequired { get; set; }
        public int NumberOfChoice { get; set; } = 1;
        public List<ProductOptionItemViewModel> ProductOptionItems { get; set; } = new List<ProductOptionItemViewModel>();
    }

    public class ProductOptionItemViewModel
    {
        public int ProductOptionItemsID { get; set; }
        public int ProductOptionGroupID { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; } = 0;
    }
}