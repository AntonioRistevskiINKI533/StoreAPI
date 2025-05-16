using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface ICustomerSaleSumsViewRepository
    {
        Task<PagedModel<CustomerSaleSumsView>> Get(int pageIndex, int pageSize, string? name, string? surname);
        //Task<IEnumerable<Book>> Get();
        //Task<Book> Get(int id);
        //Task<Book> Create(Book book);
        //Task Update(Book book);
        //Task Delete(int id);

    }
}
