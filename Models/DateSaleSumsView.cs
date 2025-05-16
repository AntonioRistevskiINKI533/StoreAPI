using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models
{
    public class DateSaleSumsView
    {
        [Key]
        public int DateId { get; set; }
        public DateTime Date { get; set; }
        public byte DayOfWeek { get; set; }
        public decimal Profit { get; set; }
        public long SumOfSales { get; set; }
        public int SumOfUnits { get; set; }
        public decimal SumOfTotalSalePrice { get; set; }
    }
}
