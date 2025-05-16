using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class BrandSaleSumsAndProfitViewService
    {
        private readonly IBrandSaleSumsAndProfitViewRepository _brandSaleSumsAndProfitViewRepository;

        public BrandSaleSumsAndProfitViewService(IBrandSaleSumsAndProfitViewRepository brandSaleSumsAndProfitViewRepository)
        {
            _brandSaleSumsAndProfitViewRepository = brandSaleSumsAndProfitViewRepository;
        }

        public async Task<PagedModel<BrandSaleSumsAndProfitViewData>> GetAll(int pageIndex, int pageSize, string? name)
        {
            var brandSaleSumsAndProfitViews = await _brandSaleSumsAndProfitViewRepository.Get(pageIndex, pageSize, name);

            var brandSaleSumsAndProfitViewsData = brandSaleSumsAndProfitViews.Items.Select(x => new BrandSaleSumsAndProfitViewData
            {
                BrandId = x.BrandId,
                Name = x.Name,
                Profit = x.Profit,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            var result = new PagedModel<BrandSaleSumsAndProfitViewData>()
            {
                TotalItems = brandSaleSumsAndProfitViews.TotalItems,
                Items = brandSaleSumsAndProfitViewsData,
            };

            return result;
        }

        //public async Task<List<BrandSaleSumsAndProfitViewData>> GetOne(int brandSaleSumsAndProfitViewId)
        //{
        //    var brandSaleSumsAndProfitViews = await _brandSaleSumsAndProfitViewRepository.GetOneBrandSaleSumsAndProfitView(brandSaleSumsAndProfitViewId);
        //
        //    var brandSaleSumsAndProfitViewsData = brandSaleSumsAndProfitViews.Select(x => new BrandSaleSumsAndProfitViewData
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
        //    return brandSaleSumsAndProfitViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertBrandSaleSumsAndProfitViewRequest request)
        //{
        //    var brandSaleSumsAndProfitView = new BrandSaleSumsAndProfitView
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
        //    var brandSaleSumsAndProfitViewRegistrationInfo = await _brandSaleSumsAndProfitViewRepository.InsertBrandSaleSumsAndProfitView(brandSaleSumsAndProfitView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(brandSaleSumsAndProfitViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = brandSaleSumsAndProfitViewRegistrationInfo.BrandSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var brandSaleSumsAndProfitView = new BrandSaleSumsAndProfitView
        //    {
        //        Id = Id,
        //    };
        //
        //    var brandSaleSumsAndProfitViewDeletionInfo = await _brandSaleSumsAndProfitViewRepository.DeleteBrandSaleSumsAndProfitView(brandSaleSumsAndProfitView.Id);
        //
        //    var idResult = new IdResponse(brandSaleSumsAndProfitViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = brandSaleSumsAndProfitViewDeletionInfo.BrandSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateBrandSaleSumsAndProfitViewRequest request)
        //{
        //    var brandSaleSumsAndProfitView = new BrandSaleSumsAndProfitView
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
        //    var brandSaleSumsAndProfitViewUpdateInfo = await _brandSaleSumsAndProfitViewRepository.UpdateBrandSaleSumsAndProfitView(brandSaleSumsAndProfitView);
        //
        //    var idResult = new IdResponse(brandSaleSumsAndProfitViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = brandSaleSumsAndProfitViewUpdateInfo.BrandSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
