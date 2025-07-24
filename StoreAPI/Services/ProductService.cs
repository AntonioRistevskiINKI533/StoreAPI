using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;
using BCrypt.Net;
using StoreAPI.Enums;
using System.ComponentModel.Design;
using StoreAPI.Exceptions;

namespace StoreAPI.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly ICompanyRepository _companyRepository;

        public ProductService(IProductRepository productRepository, IProductSaleRepository productSaleRepository, ICompanyRepository companyRepository)
        {
            _productRepository = productRepository;
            _productSaleRepository = productSaleRepository;
            _companyRepository = companyRepository;
        }

        public async Task AddProduct(AddProductRequest request)
        {
            var company = await _companyRepository.GetById(request.CompanyId);

            if (company == null)
            {
                throw new NotFoundException("Company does not exist");
            }

            var product = await _productRepository.GetByName(request.Name);

            if (product != null)
            {
                throw new ConflictException("Product with same name already exists");
            }

            var regNumExists = true;
            var registrationNumber = "";

            while (regNumExists)
            {
                registrationNumber = new Random().Next(1000000, 9999999).ToString();

                var existingProduct = await _productRepository.GetByRegistrationNumber(registrationNumber);

                if (existingProduct == null)
                {
                    regNumExists = false;
                }
            }

            product = new Product
            {
                RegistrationNumber = registrationNumber,
                Name = request.Name,
                CompanyId = request.CompanyId,
                Price = request.Price,
            };

            await _productRepository.Add(product);

            return;
        }

        public async Task UpdateProduct(UpdateProductRequest request)
        {
            var product = await _productRepository.GetById(request.Id);

            if (product == null)
            {
                throw new NotFoundException("Product does not exist");
            }

            var existingProduct = await _productRepository.GetByName(request.Name);

            if (existingProduct != null && existingProduct.Id != product.Id)
            {
                throw new ConflictException("Product with same name already exists");
            }

            if (request.CompanyId != product.CompanyId)
            {
                var company = await _companyRepository.GetById(request.CompanyId);

                if (company == null)
                {
                    throw new NotFoundException("Company does not exist");
                }
            }

            product.Name = request.Name;
            product.CompanyId = request.CompanyId;
            product.Price = request.Price;

            await _productRepository.Update(product);

            return;
        }

        public async Task<ProductData> GetProduct(int productId)
        {
            var product = await _productRepository.GetById(productId);

            if (product == null)
            {
                throw new NotFoundException("Product does not exist");
            }

            return new ProductData
            {
                Id = product.Id,
                RegistrationNumber = product.RegistrationNumber,
                Name = product.Name,
                CompanyId = product.CompanyId,
                Price = product.Price
            };
        }

        public async Task<PagedModel<ProductData>> GetAllProductsPaged(int pageIndex, int pageSize, int? companyId, string? productName)
        {
            var companies = await _productRepository.GetAllPaged(pageIndex, pageSize, companyId, productName);

            var productData = companies.Items.Select(x => new ProductData
            {
                Id = x.Id,
                RegistrationNumber = x.RegistrationNumber,
                Name = x.Name,
                CompanyId = x.CompanyId,
                Price = x.Price,
                CompanyName = x.CompanyName
            }).ToList();

            var result = new PagedModel<ProductData>()
            {
                TotalItems = companies.TotalItems,
                Items = productData
            };

            return result;
        }

        public async Task RemoveProduct(int productId)
        {
            var product = await _productRepository.GetById(productId);

            if (product == null)
            {
                throw new NotFoundException("Product does not exist");
            }

            var productSale = await _productSaleRepository.GetByProductId(productId);

            if (productSale != null)
            {
                throw new InvalidOperationException("Product has product sales, please delete them first");
            }

            await _productRepository.Remove(product);

            return;
        }
    }
}
