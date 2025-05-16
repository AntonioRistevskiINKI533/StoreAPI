using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class DatePurchaseSumsViewService
    {
        private readonly IDatePurchaseSumsViewRepository _datePurchaseSumsViewRepository;

        public DatePurchaseSumsViewService(IDatePurchaseSumsViewRepository datePurchaseSumsViewRepository)
        {
            _datePurchaseSumsViewRepository = datePurchaseSumsViewRepository;
        }

        public async Task<PagedModel<DatePurchaseSumsViewData>> GetAll(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo)
        {
            var datePurchaseSumsViews = await _datePurchaseSumsViewRepository.Get(pageIndex, pageSize, dateFrom, dateTo);

            var datePurchaseSumsViewsData = datePurchaseSumsViews.Items.Select(x => new DatePurchaseSumsViewData
            {
                DateId = x.DateId,
                Date = x.Date,
                DayOfWeek = x.DayOfWeek,
                SumOfPurchases = x.SumOfPurchases,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalPurchasePrice = x.SumOfTotalPurchasePrice,

            }).ToList();

            var result = new PagedModel<DatePurchaseSumsViewData>()
            {
                TotalItems = datePurchaseSumsViews.TotalItems,
                Items = datePurchaseSumsViewsData,
            };

            return result;
        }

        //public async Task<List<DatePurchaseSumsViewData>> GetOne(int datePurchaseSumsViewId)
        //{
        //    var datePurchaseSumsViews = await _datePurchaseSumsViewRepository.GetOneDatePurchaseSumsView(datePurchaseSumsViewId);
        //
        //    var datePurchaseSumsViewsData = datePurchaseSumsViews.Select(x => new DatePurchaseSumsViewData
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
        //    return datePurchaseSumsViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertDatePurchaseSumsViewRequest request)
        //{
        //    var datePurchaseSumsView = new DatePurchaseSumsView
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
        //    var datePurchaseSumsViewRegistrationInfo = await _datePurchaseSumsViewRepository.InsertDatePurchaseSumsView(datePurchaseSumsView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(datePurchaseSumsViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = datePurchaseSumsViewRegistrationInfo.DatePurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var datePurchaseSumsView = new DatePurchaseSumsView
        //    {
        //        Id = Id,
        //    };
        //
        //    var datePurchaseSumsViewDeletionInfo = await _datePurchaseSumsViewRepository.DeleteDatePurchaseSumsView(datePurchaseSumsView.Id);
        //
        //    var idResult = new IdResponse(datePurchaseSumsViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = datePurchaseSumsViewDeletionInfo.DatePurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateDatePurchaseSumsViewRequest request)
        //{
        //    var datePurchaseSumsView = new DatePurchaseSumsView
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
        //    var datePurchaseSumsViewUpdateInfo = await _datePurchaseSumsViewRepository.UpdateDatePurchaseSumsView(datePurchaseSumsView);
        //
        //    var idResult = new IdResponse(datePurchaseSumsViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = datePurchaseSumsViewUpdateInfo.DatePurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
