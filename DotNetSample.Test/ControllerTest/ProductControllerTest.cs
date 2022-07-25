using DotNetSample.Controllers;
using DotNetSample.Controllers.Service;
using DotNetSample.Entity;
using DotNetSample.Test.Helper;
using DotNetSample.Test.MockData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DotNetSample.Test.ControllerTest
{
    public class ProductControllerTest : BaseControllerTest
    {
        [Fact]
        public async Task GetProductsAsync_ReturnCollection()
        {
            /// Arrange
            var service = new Mock<IProductService>();
            var productList = new List<Product>() {
                FixedData.GetNewProduct(Guid.NewGuid(), "PRODUCT_1"),
                FixedData.GetNewProduct(Guid.NewGuid(), "PRODUCT_2")
            };
            service.Setup(_ => _.GetProductsAsync()).Returns(productList.AsQueryable());
            var sut = new ProductController(service.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Product>).ToList().Count.Should().Be(productList.Count());
        }

        [Fact]
        public async Task GetProductsAsync_ReturnEmptyCollection()
        {
            /// Arrange
            var service = new Mock<IProductService>();
            service.Setup(_ => _.GetProductsAsync()).Returns(new List<Product>().AsQueryable());
            var sut = new ProductController(service.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Product>).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnFound()
        {
            /// Arrange
            var service = new Mock<IProductService>();
            var productId = Guid.NewGuid();
            service.Setup(_ => _.GetProductByIdAsync(productId)).ReturnsAsync(FixedData.GetNewProduct(productId, "PRODUCT_1"));
            var sut = new ProductController(service.Object);

            /// Act
            var result = await sut.Get(productId) as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnNotFound()
        {
            /// Arrange
            var service = new Mock<IProductService>();
            var invalidId = Guid.NewGuid();
            service.Setup(_ => _.GetProductByIdAsync(invalidId)).ReturnsAsync(default(Product));
            var sut = new ProductController(service.Object);

            /// Act
            var result = await sut.Get(invalidId) as NotFoundResult;

            /// Assert
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task AddProductAsync_ReturnSuccess()
        {
            /// Arrange
            var service = new Mock<IProductService>();
            var newProductId = Guid.NewGuid();
            service.Setup(_ => _.AddProductAsync(FixedData.GetNewProduct(newProductId, "PRODUCT_NEW"))).ReturnsAsync(FixedData.GetNewProduct(newProductId, "PRODUCT_NEW"));
            var sut = new ProductController(service.Object);

            /// Act
            var result = await sut.Add(FixedData.GetNewProduct(newProductId, "PRODUCT_NEW")) as CreatedAtActionResult;

            /// Assert
            result.StatusCode.Should().Be(201);
        }
    }
}