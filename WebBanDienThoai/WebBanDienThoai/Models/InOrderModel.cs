using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanDienThoai.Models
{
    public class InOrderModel
    {
        public int? orderID { get; set; }
        public int? customerID { get; set; }
        public DateTime? CreateAt { get; set; }
        public  decimal TotalMoney { get; set; }
        public decimal SumTotal { get; set; }
    }
}