using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IProductSaleRepository
    {
        Task<ProductSale> GetByProductId(int productId);
        Task<ProductSale> Add(ProductSale productSale);
        Task<ProductSale> Update(ProductSale productSale);
        Task<PagedModel<ProductSaleData>> GetAllPaged(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo, int? productId);
        Task<ProductSale> GetById(int id);
        Task Remove(ProductSale productSale);
        Task<List<ProductSaleSumsData>> GetSums(DateTime? dateFrom, DateTime? dateTo);
    }
}
