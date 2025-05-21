using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;
using BCrypt.Net;
using StoreAPI.Enums;

namespace StoreAPI.Services
{
    public class CompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IProductRepository _productRepository;

        public CompanyService(ICompanyRepository companyRepository, IProductRepository productRepository)
        {
            _companyRepository = companyRepository;
            _productRepository = productRepository;
        }

        public async Task AddCompany(AddCompanyRequest request)
        {
            var company = await _companyRepository.GetByNameAddressOrPhone(request.Name, request.Address, request.Phone);

            if (company != null)
            {
                if (company.Name == request.Name)
                {
                    throw new Exception("Company with same name already exists");
                }

                if (company.Address == request.Address)
                {
                    throw new Exception("Company with same address already exists");
                }

                if (company.Phone == request.Phone)
                {
                    throw new Exception("Company with same phone already exists");
                }
            }

            company = new Company
            {
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone
            };

            await _companyRepository.Add(company);

            return;
        }

        public async Task UpdateCompany(UpdateCompanyRequest request)//TODO integrations tests from service to repo that check database constraints maybe???
        {
            var company = await _companyRepository.GetById(request.Id);

            if (company == null)
            {
                throw new Exception("Company does not exist");
            }

            var existingCompany = await _companyRepository.GetByNameAddressOrPhone(request.Name, request.Address, request.Phone, request.Id);

            if (existingCompany != null)
            {
                if (existingCompany.Name == request.Name)
                {
                    throw new Exception("Company with same name already exists");
                }

                if (existingCompany.Address == request.Address)
                {
                    throw new Exception("Company with same address already exists");
                }

                if (existingCompany.Phone == request.Phone)
                {
                    throw new Exception("Company with same phone already exists");
                }
            }

            company.Name = request.Name;
            company.Address = request.Address;
            company.Phone = request.Phone;

            await _companyRepository.Update(company);

            return;
        }

        public async Task<CompanyData> GetCompany(int companyId)
        {
            var company = await _companyRepository.GetById(companyId);

            if (company == null)
            {
                throw new Exception("Company does not exist");
            }

            return new CompanyData
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                Phone = company.Phone
            };
        }

        public async Task<PagedModel<CompanyData>> GetAllCompaniesPaged(int pageIndex, int pageSize)
        {
            var companies = await _companyRepository.GetAllPaged(pageIndex, pageSize);

            var companyData = companies.Items.Select(x => new CompanyData
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone
            }).ToList();

            var result = new PagedModel<CompanyData>()
            {
                TotalItems = companies.TotalItems,
                Items = companyData
            };

            return result;
        }

        public async Task RemoveCompany(int companyId)
        {
            var company = await _companyRepository.GetById(companyId);

            if (company == null)
            {
                throw new Exception("Company does not exist");
            }

            var products = await _productRepository.GetByCompanyId(companyId);

            if (products != null)
            {
                throw new Exception("Company has products, please delete them first");
            }

            await _companyRepository.Remove(company);

            return;
        }
    }
}
