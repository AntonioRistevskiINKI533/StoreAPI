using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class SupplierPurchaseSumsViewService
    {
        private readonly ISupplierPurchaseSumsViewRepository _supplierPurchaseSumsViewRepository;

        public SupplierPurchaseSumsViewService(ISupplierPurchaseSumsViewRepository supplierPurchaseSumsViewRepository)
        {
            _supplierPurchaseSumsViewRepository = supplierPurchaseSumsViewRepository;
        }

        public async Task<PagedModel<SupplierPurchaseSumsViewData>> GetAll(int pageIndex, int pageSize, string? name)
        {
            var supplierPurchaseSumsViews = await _supplierPurchaseSumsViewRepository.Get(pageIndex, pageSize, name);

            var supplierPurchaseSumsViewsData = supplierPurchaseSumsViews.Items.Select(x => new SupplierPurchaseSumsViewData
            {
                SupplierId = x.SupplierId,
                Name = x.Name,
                Email = x.Email,
                SumOfPurchases = x.SumOfPurchases,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalPurchasePrice = x.SumOfTotalPurchasePrice,

            }).ToList();

            var result = new PagedModel<SupplierPurchaseSumsViewData>()
            {
                TotalItems = supplierPurchaseSumsViews.TotalItems,
                Items = supplierPurchaseSumsViewsData,
            };

            return result;
        }

        //public async Task<List<SupplierPurchaseSumsViewData>> GetOne(int supplierPurchaseSumsViewId)
        //{
        //    var supplierPurchaseSumsViews = await _supplierPurchaseSumsViewRepository.GetOneSupplierPurchaseSumsView(supplierPurchaseSumsViewId);
        //
        //    var supplierPurchaseSumsViewsData = supplierPurchaseSumsViews.Select(x => new SupplierPurchaseSumsViewData
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
        //    return supplierPurchaseSumsViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertSupplierPurchaseSumsViewRequest request)
        //{
        //    var supplierPurchaseSumsView = new SupplierPurchaseSumsView
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
        //    var supplierPurchaseSumsViewRegistrationInfo = await _supplierPurchaseSumsViewRepository.InsertSupplierPurchaseSumsView(supplierPurchaseSumsView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(supplierPurchaseSumsViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = supplierPurchaseSumsViewRegistrationInfo.SupplierPurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var supplierPurchaseSumsView = new SupplierPurchaseSumsView
        //    {
        //        Id = Id,
        //    };
        //
        //    var supplierPurchaseSumsViewDeletionInfo = await _supplierPurchaseSumsViewRepository.DeleteSupplierPurchaseSumsView(supplierPurchaseSumsView.Id);
        //
        //    var idResult = new IdResponse(supplierPurchaseSumsViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = supplierPurchaseSumsViewDeletionInfo.SupplierPurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateSupplierPurchaseSumsViewRequest request)
        //{
        //    var supplierPurchaseSumsView = new SupplierPurchaseSumsView
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
        //    var supplierPurchaseSumsViewUpdateInfo = await _supplierPurchaseSumsViewRepository.UpdateSupplierPurchaseSumsView(supplierPurchaseSumsView);
        //
        //    var idResult = new IdResponse(supplierPurchaseSumsViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = supplierPurchaseSumsViewUpdateInfo.SupplierPurchaseSumsViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
