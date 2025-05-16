using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class ProductSaleSumsAndProfitViewService
    {
        private readonly IProductSaleSumsAndProfitViewRepository _productSaleSumsAndProfitViewRepository;

        public ProductSaleSumsAndProfitViewService(IProductSaleSumsAndProfitViewRepository productSaleSumsAndProfitViewRepository)
        {
            _productSaleSumsAndProfitViewRepository = productSaleSumsAndProfitViewRepository;
        }

        public async Task<PagedModel<ProductSaleSumsAndProfitViewData>> GetAll(int pageIndex, int pageSize, string? name)
        {
            var productSaleSumsAndProfitViews = await _productSaleSumsAndProfitViewRepository.Get(pageIndex, pageSize, name);

            var productSaleSumsAndProfitViewsData = productSaleSumsAndProfitViews.Items.Select(x => new ProductSaleSumsAndProfitViewData
            {
                ProductId = x.ProductId,
                Name = x.Name,
                Profit = x.Profit,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            var result = new PagedModel<ProductSaleSumsAndProfitViewData>()
            {
                TotalItems = productSaleSumsAndProfitViews.TotalItems,
                Items = productSaleSumsAndProfitViewsData,
            };

            return result;
        }

        //public async Task<List<ProductSaleSumsAndProfitViewData>> GetOne(int productSaleSumsAndProfitViewId)
        //{
        //    var productSaleSumsAndProfitViews = await _productSaleSumsAndProfitViewRepository.GetOneProductSaleSumsAndProfitView(productSaleSumsAndProfitViewId);
        //
        //    var productSaleSumsAndProfitViewsData = productSaleSumsAndProfitViews.Select(x => new ProductSaleSumsAndProfitViewData
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
        //    return productSaleSumsAndProfitViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertProductSaleSumsAndProfitViewRequest request)
        //{
        //    var productSaleSumsAndProfitView = new ProductSaleSumsAndProfitView
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
        //    var productSaleSumsAndProfitViewRegistrationInfo = await _productSaleSumsAndProfitViewRepository.InsertProductSaleSumsAndProfitView(productSaleSumsAndProfitView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(productSaleSumsAndProfitViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productSaleSumsAndProfitViewRegistrationInfo.ProductSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var productSaleSumsAndProfitView = new ProductSaleSumsAndProfitView
        //    {
        //        Id = Id,
        //    };
        //
        //    var productSaleSumsAndProfitViewDeletionInfo = await _productSaleSumsAndProfitViewRepository.DeleteProductSaleSumsAndProfitView(productSaleSumsAndProfitView.Id);
        //
        //    var idResult = new IdResponse(productSaleSumsAndProfitViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productSaleSumsAndProfitViewDeletionInfo.ProductSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateProductSaleSumsAndProfitViewRequest request)
        //{
        //    var productSaleSumsAndProfitView = new ProductSaleSumsAndProfitView
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
        //    var productSaleSumsAndProfitViewUpdateInfo = await _productSaleSumsAndProfitViewRepository.UpdateProductSaleSumsAndProfitView(productSaleSumsAndProfitView);
        //
        //    var idResult = new IdResponse(productSaleSumsAndProfitViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productSaleSumsAndProfitViewUpdateInfo.ProductSaleSumsAndProfitViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
