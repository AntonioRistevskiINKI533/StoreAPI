using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;

namespace StoreAPI.Services
{
    public class CustomerSaleSumsViewService
    {
        private readonly ICustomerSaleSumsViewRepository _customerSaleSumsViewRepository;

        public CustomerSaleSumsViewService(ICustomerSaleSumsViewRepository customerSaleSumsViewRepository)
        {
            _customerSaleSumsViewRepository = customerSaleSumsViewRepository;
        }

        public async Task<PagedModel<CustomerSaleSumsViewData>> GetAll(int pageIndex, int pageSize, string? name, string? surname)
        {
            var customerSaleSumsViews = await _customerSaleSumsViewRepository.Get(pageIndex, pageSize, name, surname);

            var customerSaleSumsViewsData = customerSaleSumsViews.Items.Select(x => new CustomerSaleSumsViewData
            {
                CustomerId = x.CustomerId,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email,
                Profit = x.Profit,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            var result = new PagedModel<CustomerSaleSumsViewData>()
            {
                TotalItems = customerSaleSumsViews.TotalItems,
                Items = customerSaleSumsViewsData,
            };

            return result;
        }

        //public async Task<List<CustomerSaleSumsViewData>> GetOne(int customerSaleSumsViewId)
        //{
        //    var customerSaleSumsViews = await _customerSaleSumsViewRepository.GetOneCustomerSaleSumsView(customerSaleSumsViewId);
        //
        //    var customerSaleSumsViewsData = customerSaleSumsViews.Select(x => new CustomerSaleSumsViewData
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
        //    return customerSaleSumsViewsData;
        //}
        //
        //public async Task<IdResponse> Insert(InsertCustomerSaleSumsViewRequest request)
        //{
        //    var customerSaleSumsView = new CustomerSaleSumsView
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
        //    var customerSaleSumsViewRegistrationInfo = await _customerSaleSumsViewRepository.InsertCustomerSaleSumsView(customerSaleSumsView, request.ImplementedProgramId);
        //
        //    var idResult = new IdResponse(customerSaleSumsViewRegistrationInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = customerSaleSumsViewRegistrationInfo.CustomerSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Delete(int Id)
        //{
        //    var customerSaleSumsView = new CustomerSaleSumsView
        //    {
        //        Id = Id,
        //    };
        //
        //    var customerSaleSumsViewDeletionInfo = await _customerSaleSumsViewRepository.DeleteCustomerSaleSumsView(customerSaleSumsView.Id);
        //
        //    var idResult = new IdResponse(customerSaleSumsViewDeletionInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = customerSaleSumsViewDeletionInfo.CustomerSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
        //
        //public async Task<IdResponse> Update(UpdateCustomerSaleSumsViewRequest request)
        //{
        //    var customerSaleSumsView = new CustomerSaleSumsView
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
        //    var customerSaleSumsViewUpdateInfo = await _customerSaleSumsViewRepository.UpdateCustomerSaleSumsView(customerSaleSumsView);
        //
        //    var idResult = new IdResponse(customerSaleSumsViewUpdateInfo.StatusCode.ToResultStatus())
        //    {
        //        Id = customerSaleSumsViewUpdateInfo.CustomerSaleSumsViewId,
        //    };
        //
        //    return idResult;
        //}
    }
}
