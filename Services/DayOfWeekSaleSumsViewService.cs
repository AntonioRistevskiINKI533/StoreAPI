using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class DayOfWeekSaleSumsViewService
    {
        private readonly IDayOfWeekSaleSumsViewRepository _dateSaleSumsViewRepository;

        public DayOfWeekSaleSumsViewService(IDayOfWeekSaleSumsViewRepository dateSaleSumsViewRepository)
        {
            _dateSaleSumsViewRepository = dateSaleSumsViewRepository;
        }

        public async Task<PagedModel<DayOfWeekSaleSumsViewData>> GetAll(int pageIndex, int pageSize)
        {
            var dateSaleSumsViews = await _dateSaleSumsViewRepository.Get(pageIndex, pageSize);

            var dateSaleSumsViewsData = dateSaleSumsViews.Items.Select(x => new DayOfWeekSaleSumsViewData
            {
                DayOfWeek = x.DayOfWeek,
                Profit = x.Profit,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            var result = new PagedModel<DayOfWeekSaleSumsViewData>()
            {
                TotalItems = dateSaleSumsViews.TotalItems,
                Items = dateSaleSumsViewsData,
            };

            return result;
        }

        //public async Task<List<DayOfWeekSaleSumsViewData>> GetOne(int dateSaleSumsViewId)
        //{
        //    var dateSaleSumsViews = await _dateSaleSumsViewRepository.GetOneDayOfWeekSaleSumsView(dateSaleSumsViewId);
        //
        //    var dateSaleSumsViewsData = dateSaleSumsViews.Select(x => new DayOfWeekSaleSumsViewData
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Surname = x.Surname,
        //        DayOfWeekOfBirth = x.DayOfWeekOfBirth,
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
        //public async Task<IdResponse> Insert(InsertDayOfWeekSaleSumsViewRequest request)
        //{
        //    var dateSaleSumsView = new DayOfWeekSaleSumsView
        //    {
        //        Name = request.Name,
        //        Surname = request.Surname,
        //        DayOfWeekOfBirth = request.DayOfWeekOfBirth,
        //        PlaceOfBirth = request.PlaceOfBirth,
        //        WorkplaceCode = request.WorkplaceCode,
        //        Institution = request.Institution,
        //        PlaceMunicipality = request.PlaceMunicipality,
        //    };
        //
        //
        //    var dateSaleSumsViewRegistrationInfo = await _dateSaleSumsViewRepository.InsertDayOfWeekSaleSumsView(dateSaleSumsView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(dateSaleSumsViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = dateSaleSumsViewRegistrationInfo.DayOfWeekSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var dateSaleSumsView = new DayOfWeekSaleSumsView
        //    {
        //        Id = Id,
        //    };
        //
        //    var dateSaleSumsViewDeletionInfo = await _dateSaleSumsViewRepository.DeleteDayOfWeekSaleSumsView(dateSaleSumsView.Id);
        //
        //    var idResult = new IdResponse(dateSaleSumsViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = dateSaleSumsViewDeletionInfo.DayOfWeekSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateDayOfWeekSaleSumsViewRequest request)
        //{
        //    var dateSaleSumsView = new DayOfWeekSaleSumsView
        //    {
        //        Id = request.Id,
        //        Name = request.Name,
        //        Surname = request.Surname,
        //        DayOfWeekOfBirth = request.DayOfWeekOfBirth,
        //        PlaceOfBirth = request.PlaceOfBirth,
        //        WorkplaceCode = request.WorkplaceCode,
        //        Institution = request.Institution,
        //        PlaceMunicipality = request.PlaceMunicipality,
        //    };
        //
        //    var dateSaleSumsViewUpdateInfo = await _dateSaleSumsViewRepository.UpdateDayOfWeekSaleSumsView(dateSaleSumsView);
        //
        //    var idResult = new IdResponse(dateSaleSumsViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = dateSaleSumsViewUpdateInfo.DayOfWeekSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
