using StoreAPI.Models;
using StoreAPI.Models.Datas;

namespace StoreAPI.Repositories.Interfaces
{
    public interface IDayOfWeekSaleSumsViewRepository
    {
        Task<PagedModel<DayOfWeekSaleSumsView>> Get(int pageIndex, int pageSize);
        //Task<IEnumerable<Book>> Get();
        //Task<Book> Get(int id);
        //Task<Book> Create(Book book);
        //Task Update(Book book);
        //Task Delete(int id);

    }
}
