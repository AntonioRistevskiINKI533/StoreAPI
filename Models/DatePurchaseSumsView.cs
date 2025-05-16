using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models
{
    public class DatePurchaseSumsView
    {
        [Key]
        public int DateId { get; set; }
        public DateTime Date { get; set; }
        public byte DayOfWeek { get; set; }
        public long SumOfPurchases { get; set; }
        public int SumOfUnits { get; set; }
        public decimal SumOfTotalPurchasePrice { get; set; }
    }
}
