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
    public class CategoryTest : BaseIntegrationTest
    {
        static int categoryCount;

        static Func<AppDbContext, bool> seed = (db) =>
        {
            // Seed Categories
            categoryCount = 2;
            db.Categories.AddRange(SeedData.WhiteCategory, SeedData.BlackCategory);

            db.SaveChangesAsync();

            return true;
        };

        public CategoryTest() : base("Test", seed) { }

        [Fact]
        public async Task GetCategoriesAsync_ReturnCollection()
        {
            /// Act
            var result = await TestClient.GetAsync<List<Category>>("/category");

            /// Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(categoryCount);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnFound()
        {
            /// Arrange 
            var id = SeedData.WhiteCategory.Id;

            /// Act
            var result = await TestClient.GetAsync<Category>($"/category/{id}");

            /// Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnNotFound()
        {
            /// Act
            var result = await TestClient.GetAsync<Category>($"/category/{Guid.NewGuid()}");

            /// Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddCategoryAsync_ReturnSuccess()
        {
            /// Arrange
            var newCategory = await TestClient.PostAsyncAndReturn<Category, Category>("/category", FixedData.GetNewCategory(Guid.NewGuid(), "CATEGORY_NEW"));

            /// Act
            var result = await TestClient.GetAsync<Category>($"/category/{newCategory.Id}");

            /// Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(newCategory.Id);
        }
    }
}
