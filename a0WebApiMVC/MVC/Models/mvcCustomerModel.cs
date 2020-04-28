using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class mvcCustomerModel
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }
        public Nullable<int> InvoiceID { get; set; }
    }
}