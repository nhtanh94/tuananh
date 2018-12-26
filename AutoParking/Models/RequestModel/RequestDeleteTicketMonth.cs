using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoParking.Models.RequestModel
{
    public class RequestDeleteTicketMonth
    {
        [Required]
        public string Code { set; get; }
        [Required]
        public int rowID { set; get; }
    }
}