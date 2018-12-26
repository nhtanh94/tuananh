using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoParking.Models.RequestModel
{
    public class RequestSearchTicketMonthExpires
    {
        [Required]
        public string Code { get; set; }
        public string SearchString { get; set; }
    }
}