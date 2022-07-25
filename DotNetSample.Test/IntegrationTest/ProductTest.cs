using DotNetSample.Data;
using DotNetSample.Entity;
using DotNetSample.Test.Helper;
using DotNetSample.Test.MockData;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DotNetSample.Test.IntegrationTest
{
    public class ProductTest : BaseIntegrationTest
    {
        static int productCount;

        static Func<AppDbContext, bool> seed = (db) =>
        {
            // Seed Products
            productCount = 2;
            db.Products.AddRange(SeedData.IPhoneX, SeedData.S20);

            db.SaveChangesAsync();

            return true;
        };

        public ProductTest() : base("Test", seed) { }

        [Fact]
        public async Task GetProductsAsync_ReturnCollection()
        {
            /// Act
            var result = await TestClient.GetAsync<List<Product>>("/product");

            /// Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(productCount);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnFound()
        {
            /// Arrange 
            var id = SeedData.IPhoneX.Id;

            /// Act
            var result = await TestClient.GetAsync<Product>($"/product/{id}");

            /// Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnNotFound()
        {
            /// Act
            var result = await TestClient.GetAsync<Product>($"/product/{Guid.NewGuid()}");

            /// Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddProductAsync_ReturnSuccess()
        {
            /// Arrange
            var newProduct = await TestClient.PostAsyncAndReturn<Product, Product>("/product", FixedData.GetNewProduct(Guid.NewGuid(), "PRODUCT_NEW"));

            /// Act
            var result = await TestClient.GetAsync<Product>($"/product/{newProduct.Id}");

            /// Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(newProduct.Id);
        }
    }
}
