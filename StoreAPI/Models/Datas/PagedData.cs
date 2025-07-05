using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StoreAPI.Models.Datas
{
    public class PagedModel<IModel>
    {
        public int TotalItems { get; set; }
        public IList<IModel> Items { get; set; }

        public PagedModel()
        {
            Items = new List<IModel>();
        }
    }
}
