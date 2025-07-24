using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;
using BCrypt.Net;
using StoreAPI.Enums;
using StoreAPI.Exceptions;

namespace StoreAPI.Services
{
    public class ProductSaleService
    {
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly IProductRepository _productRepository;

        public ProductSaleService(IProductSaleRepository productSaleRepository, IProductRepository productRepository)
        {
            _productSaleRepository = productSaleRepository;
            _productRepository = productRepository;
        }

        public async Task AddProductSale(AddProductSaleRequest request)
        {
            //TODO if stock in product is added i will need to check if we have in stock enough, or maybe not... since the stock was from tomorrow hm.... i will have to take wayt from stock idk. or add options to take or not take
            var product = await _productRepository.GetById(request.ProductId);

            if (product == null)
            {
                throw new NotFoundException("Product does not exist");
            }

            if (request.PricePerUnit == null)
            {
                request.PricePerUnit = product.Price;
            }

            var productSale = new ProductSale
            {
                ProductId = request.ProductId,
                SoldAmount = request.SoldAmount,
                PricePerUnit = (decimal)request.PricePerUnit,
                Date = request.Date ?? DateTime.Now,
            };

            await _productSaleRepository.Add(productSale);

            return;
        }

        public async Task UpdateProductSale(UpdateProductSaleRequest request)
        {
            var productSale = await _productSaleRepository.GetById(request.Id);

            if (productSale == null)
            {
                throw new NotFoundException("Product sale does not exist");
            }

            if (request.ProductId != productSale.ProductId)
            {
                var product = await _productRepository.GetById(request.ProductId);

                if (product == null)
                {
                    throw new NotFoundException("Product does not exist");
                }
            }

            productSale.ProductId = request.ProductId;
            productSale.SoldAmount = request.SoldAmount;
            productSale.PricePerUnit = request.PricePerUnit;
            productSale.Date = request.Date;

            await _productSaleRepository.Update(productSale);

            return;
        }

        public async Task<ProductSaleData> GetProductSale(int productSaleId)
        {
            var productSale = await _productSaleRepository.GetById(productSaleId);

            if (productSale == null)
            {
                throw new NotFoundException("Product sale does not exist");
            }

            return new ProductSaleData
            {
                Id = productSale.Id,
                ProductId = productSale.ProductId,
                SoldAmount = productSale.SoldAmount,
                PricePerUnit = productSale.PricePerUnit,
                Date = productSale.Date
            };
        }

        public async Task<PagedModel<ProductSaleData>> GetAllProductSalesPaged(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo, int? productId)
        {
            var companies = await _productSaleRepository.GetAllPaged(pageIndex, pageSize, dateFrom, dateTo, productId);

            var productSaleData = companies.Items.Select(x => new ProductSaleData
            {
                Id = x.Id,
                ProductId = x.ProductId,
                SoldAmount = x.SoldAmount,
                PricePerUnit = x.PricePerUnit,
                Date = x.Date,
                ProductName = x.ProductName,
            }).ToList();

            var result = new PagedModel<ProductSaleData>()
            {
                TotalItems = companies.TotalItems,
                Items = productSaleData
            };

            return result;
        }

        public async Task RemoveProductSale(int productSaleId)
        {
            var productSale = await _productSaleRepository.GetById(productSaleId);

            if (productSale == null)
            {
                throw new NotFoundException("Product sale does not exist");
            }

            await _productSaleRepository.Remove(productSale);

            return;
        }

        public async Task<List<ProductSaleSumsData>> GetSums(DateTime? dateFrom, DateTime? dateTo)
        {
            var productSaleSums = await _productSaleRepository.GetSums(dateFrom, dateTo);

            var productSaleSumsData = productSaleSums.Select(x => new ProductSaleSumsData
            {
                ProductId = x.ProductId,
                Name = x.Name,
                SumOfSales = x.SumOfSales,
                SumOfUnits = x.SumOfUnits,
                SumOfTotalSalePrice = x.SumOfTotalSalePrice,

            }).ToList();

            return productSaleSumsData;
        }
    }
}
