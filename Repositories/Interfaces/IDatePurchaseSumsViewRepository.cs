using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IDatePurchaseSumsViewRepository
    {
        Task<PagedModel<DatePurchaseSumsView>> Get(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo);
        //Task<IEnumerable<Book>> Get();
        //Task<Book> Get(int id);
        //Task<Book> Create(Book book);
        //Task Update(Book book);
        //Task Delete(int id);

    }
}
