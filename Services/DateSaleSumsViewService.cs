using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class DateSaleSumsViewService
    {
        private readonly IDateSaleSumsViewRepository _dateSaleSumsViewRepository;

        public DateSaleSumsViewService(IDateSaleSumsViewRepository dateSaleSumsViewRepository)
        {
            _dateSaleSumsViewRepository = dateSaleSumsViewRepository;
        }

        public async Task<PagedModel<DateSaleSumsViewData>> GetAll(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo)
        {
            var dateSaleSumsViews = await _dateSaleSumsViewRepository.Get(pageIndex, pageSize, dateFrom, dateTo);

            var dateSaleSumsViewsData = dateSaleSumsViews.Items.Select(x => new DateSaleSumsViewData
            {
                DateId = x.DateId,
                Date = x.Date,
                DayOfWeek = x.DayOfWeek,
                Profit = x.Profit,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            var result = new PagedModel<DateSaleSumsViewData>()
            {
                TotalItems = dateSaleSumsViews.TotalItems,
                Items = dateSaleSumsViewsData,
            };

            return result;
        }

        //public async Task<List<DateSaleSumsViewData>> GetOne(int dateSaleSumsViewId)
        //{
        //    var dateSaleSumsViews = await _dateSaleSumsViewRepository.GetOneDateSaleSumsView(dateSaleSumsViewId);
        //
        //    var dateSaleSumsViewsData = dateSaleSumsViews.Select(x => new DateSaleSumsViewData
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Surname = x.Surname,
        //        DateOfBirth = x.DateOfBirth,
        //        PlaceOfBirth = x.PlaceOfBirth,
        //        WorkplaceCode = x.WorkplaceCode,
        //        Institution = x.Institution,
        //        PlaceMunicipality = x.PlaceMunicipality,
        //
        //    }).ToList();
        //
        //    return dateSaleSumsViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertDateSaleSumsViewRequest request)
        //{
        //    var dateSaleSumsView = new DateSaleSumsView
        //    {
        //        Name = request.Name,
        //        Surname = request.Surname,
        //        DateOfBirth = request.DateOfBirth,
        //        PlaceOfBirth = request.PlaceOfBirth,
        //        WorkplaceCode = request.WorkplaceCode,
        //        Institution = request.Institution,
        //        PlaceMunicipality = request.PlaceMunicipality,
        //    };
        //
        //
        //    var dateSaleSumsViewRegistrationInfo = await _dateSaleSumsViewRepository.InsertDateSaleSumsView(dateSaleSumsView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(dateSaleSumsViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = dateSaleSumsViewRegistrationInfo.DateSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var dateSaleSumsView = new DateSaleSumsView
        //    {
        //        Id = Id,
        //    };
        //
        //    var dateSaleSumsViewDeletionInfo = await _dateSaleSumsViewRepository.DeleteDateSaleSumsView(dateSaleSumsView.Id);
        //
        //    var idResult = new IdResponse(dateSaleSumsViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = dateSaleSumsViewDeletionInfo.DateSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateDateSaleSumsViewRequest request)
        //{
        //    var dateSaleSumsView = new DateSaleSumsView
        //    {
        //        Id = request.Id,
        //        Name = request.Name,
        //        Surname = request.Surname,
        //        DateOfBirth = request.DateOfBirth,
        //        PlaceOfBirth = request.PlaceOfBirth,
        //        WorkplaceCode = request.WorkplaceCode,
        //        Institution = request.Institution,
        //        PlaceMunicipality = request.PlaceMunicipality,
        //    };
        //
        //    var dateSaleSumsViewUpdateInfo = await _dateSaleSumsViewRepository.UpdateDateSaleSumsView(dateSaleSumsView);
        //
        //    var idResult = new IdResponse(dateSaleSumsViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = dateSaleSumsViewUpdateInfo.DateSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
