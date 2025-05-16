using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IProductTypeSaleSumsAndProfitViewRepository
    {
        Task<PagedModel<ProductTypeSaleSumsAndProfitView>> Get(int pageIndex, int pageSize, string? name);
        //Task<IEnumerable<Book>> Get();
        //Task<Book> Get(int id);
        //Task<Book> Create(Book book);
        //Task Update(Book book);
        //Task Delete(int id);

    }
}
