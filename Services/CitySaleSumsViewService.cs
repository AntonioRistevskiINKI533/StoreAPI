using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class CitySaleSumsViewService
    {
        private readonly ICitySaleSumsViewRepository _citySaleSumsViewRepository;

        public CitySaleSumsViewService(ICitySaleSumsViewRepository citySaleSumsViewRepository)
        {
            _citySaleSumsViewRepository = citySaleSumsViewRepository;
        }

        public async Task<PagedModel<CitySaleSumsViewData>> GetAll(int pageIndex, int pageSize, string? name)
        {
            var citySaleSumsViews = await _citySaleSumsViewRepository.Get(pageIndex, pageSize, name);

            var citySaleSumsViewsData = citySaleSumsViews.Items.Select(x => new CitySaleSumsViewData
            {
                CityId = x.CityId,
                Name = x.Name,
                Profit = x.Profit,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            var result = new PagedModel<CitySaleSumsViewData>()
            {
                TotalItems = citySaleSumsViews.TotalItems,
                Items = citySaleSumsViewsData,
            };

            return result;
        }

        //public async Task<List<CitySaleSumsViewData>> GetOne(int citySaleSumsViewId)
        //{
        //    var citySaleSumsViews = await _citySaleSumsViewRepository.GetOneCitySaleSumsView(citySaleSumsViewId);
        //
        //    var citySaleSumsViewsData = citySaleSumsViews.Select(x => new CitySaleSumsViewData
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
        //    return citySaleSumsViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertCitySaleSumsViewRequest request)
        //{
        //    var citySaleSumsView = new CitySaleSumsView
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
        //    var citySaleSumsViewRegistrationInfo = await _citySaleSumsViewRepository.InsertCitySaleSumsView(citySaleSumsView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(citySaleSumsViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = citySaleSumsViewRegistrationInfo.CitySaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var citySaleSumsView = new CitySaleSumsView
        //    {
        //        Id = Id,
        //    };
        //
        //    var citySaleSumsViewDeletionInfo = await _citySaleSumsViewRepository.DeleteCitySaleSumsView(citySaleSumsView.Id);
        //
        //    var idResult = new IdResponse(citySaleSumsViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = citySaleSumsViewDeletionInfo.CitySaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateCitySaleSumsViewRequest request)
        //{
        //    var citySaleSumsView = new CitySaleSumsView
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
        //    var citySaleSumsViewUpdateInfo = await _citySaleSumsViewRepository.UpdateCitySaleSumsView(citySaleSumsView);
        //
        //    var idResult = new IdResponse(citySaleSumsViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = citySaleSumsViewUpdateInfo.CitySaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
