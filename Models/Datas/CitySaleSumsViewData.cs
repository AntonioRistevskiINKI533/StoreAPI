using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Datas
{
    public class CitySaleSumsViewData
    {
        public int CityId { get; set; }
        public string? Name { get; set; }
        public decimal Profit { get; set; }
        public long SumOfSales { get; set; }
        public int SumOfUnits { get; set; }
        public decimal SumOfTotalSalePrice { get; set; }
    }
}
