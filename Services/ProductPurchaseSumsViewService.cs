using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class ProductPurchaseSumsViewService
    {
        private readonly IProductPurchaseSumsViewRepository _productPurchaseSumsViewRepository;

        public ProductPurchaseSumsViewService(IProductPurchaseSumsViewRepository productPurchaseSumsViewRepository)
        {
            _productPurchaseSumsViewRepository = productPurchaseSumsViewRepository;
        }

        public async Task<PagedModel<ProductPurchaseSumsViewData>> GetAll(int pageIndex, int pageSize, string? name)
        {
            var productPurchaseSumsViews = await _productPurchaseSumsViewRepository.Get(pageIndex, pageSize, name);

            var productPurchaseSumsViewsData = productPurchaseSumsViews.Items.Select(x => new ProductPurchaseSumsViewData
            {
                ProductId = x.ProductId,
                Name = x.Name,
                SumOfPurchases = x.SumOfPurchases,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalPurchasePrice = x.SumOfTotalPurchasePrice,

            }).ToList();

            var result = new PagedModel<ProductPurchaseSumsViewData>()
            {
                TotalItems = productPurchaseSumsViews.TotalItems,
                Items = productPurchaseSumsViewsData,
            };

            return result;
        }

        //public async Task<List<ProductPurchaseSumsViewData>> GetOne(int productPurchaseSumsViewId)
        //{
        //    var productPurchaseSumsViews = await _productPurchaseSumsViewRepository.GetOneProductPurchaseSumsView(productPurchaseSumsViewId);
        //
        //    var productPurchaseSumsViewsData = productPurchaseSumsViews.Select(x => new ProductPurchaseSumsViewData
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
        //    return productPurchaseSumsViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertProductPurchaseSumsViewRequest request)
        //{
        //    var productPurchaseSumsView = new ProductPurchaseSumsView
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
        //    var productPurchaseSumsViewRegistrationInfo = await _productPurchaseSumsViewRepository.InsertProductPurchaseSumsView(productPurchaseSumsView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(productPurchaseSumsViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productPurchaseSumsViewRegistrationInfo.ProductPurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var productPurchaseSumsView = new ProductPurchaseSumsView
        //    {
        //        Id = Id,
        //    };
        //
        //    var productPurchaseSumsViewDeletionInfo = await _productPurchaseSumsViewRepository.DeleteProductPurchaseSumsView(productPurchaseSumsView.Id);
        //
        //    var idResult = new IdResponse(productPurchaseSumsViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productPurchaseSumsViewDeletionInfo.ProductPurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateProductPurchaseSumsViewRequest request)
        //{
        //    var productPurchaseSumsView = new ProductPurchaseSumsView
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
        //    var productPurchaseSumsViewUpdateInfo = await _productPurchaseSumsViewRepository.UpdateProductPurchaseSumsView(productPurchaseSumsView);
        //
        //    var idResult = new IdResponse(productPurchaseSumsViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = productPurchaseSumsViewUpdateInfo.ProductPurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
