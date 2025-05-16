using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class ProductTypeSaleSumsAndProfitViewService
    {
        private readonly IProductTypeSaleSumsAndProfitViewRepository _productTypeSaleSumsAndProfitViewRepository;

        public ProductTypeSaleSumsAndProfitViewService(IProductTypeSaleSumsAndProfitViewRepository productTypeSaleSumsAndProfitViewRepository)
        {
            _productTypeSaleSumsAndProfitViewRepository = productTypeSaleSumsAndProfitViewRepository;
        }

        public async Task<PagedModel<ProductTypeSaleSumsAndProfitViewData>> GetAll(int pageIndex, int pageSize, string? name)
        {
            var productTypeSaleSumsAndProfitViews = await _productTypeSaleSumsAndProfitViewRepository.Get(pageIndex, pageSize, name);

            var productTypeSaleSumsAndProfitViewsData = productTypeSaleSumsAndProfitViews.Items.Select(x => new ProductTypeSaleSumsAndProfitViewData
            {
                ProductTypeId = x.ProductTypeId,
                Name = x.Name,
                Profit = x.Profit,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            var result = new PagedModel<ProductTypeSaleSumsAndProfitViewData>()
            {
                TotalItems = productTypeSaleSumsAndProfitViews.TotalItems,
                Items = productTypeSaleSumsAndProfitViewsData,
            };

            return result;
        }

        //public async Task<List<ProductTypeSaleSumsAndProfitViewData>> GetOne(int productTypeSaleSumsAndProfitViewId)
        //{
        //    var productTypeSaleSumsAndProfitViews = await _productTypeSaleSumsAndProfitViewRepository.GetOneProductTypeSaleSumsAndProfitView(productTypeSaleSumsAndProfitViewId);
        //
        //    var productTypeSaleSumsAndProfitViewsData = productTypeSaleSumsAndProfitViews.Select(x => new ProductTypeSaleSumsAndProfitViewData
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
        //    return productTypeSaleSumsAndProfitViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertProductTypeSaleSumsAndProfitViewRequest request)
        //{
        //    var productTypeSaleSumsAndProfitView = new ProductTypeSaleSumsAndProfitView
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
        //    var productTypeSaleSumsAndProfitViewRegistrationInfo = await _productTypeSaleSumsAndProfitViewRepository.InsertProductTypeSaleSumsAndProfitView(productTypeSaleSumsAndProfitView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(productTypeSaleSumsAndProfitViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productTypeSaleSumsAndProfitViewRegistrationInfo.ProductTypeSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var productTypeSaleSumsAndProfitView = new ProductTypeSaleSumsAndProfitView
        //    {
        //        Id = Id,
        //    };
        //
        //    var productTypeSaleSumsAndProfitViewDeletionInfo = await _productTypeSaleSumsAndProfitViewRepository.DeleteProductTypeSaleSumsAndProfitView(productTypeSaleSumsAndProfitView.Id);
        //
        //    var idResult = new IdResponse(productTypeSaleSumsAndProfitViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productTypeSaleSumsAndProfitViewDeletionInfo.ProductTypeSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateProductTypeSaleSumsAndProfitViewRequest request)
        //{
        //    var productTypeSaleSumsAndProfitView = new ProductTypeSaleSumsAndProfitView
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
        //    var productTypeSaleSumsAndProfitViewUpdateInfo = await _productTypeSaleSumsAndProfitViewRepository.UpdateProductTypeSaleSumsAndProfitView(productTypeSaleSumsAndProfitView);
        //
        //    var idResult = new IdResponse(productTypeSaleSumsAndProfitViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productTypeSaleSumsAndProfitViewUpdateInfo.ProductTypeSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
