using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IProductSaleRepository
    {
        Task<ProductSale> Add(ProductSale productSale);
        Task<ProductSale> Update(ProductSale productSale);
        Task<PagedModel<ProductSale>> GetAllPaged(int pageIndex, int pageSize);
        Task<ProductSale> GetById(int id);
        Task Remove(ProductSale productSale);
    }
}
