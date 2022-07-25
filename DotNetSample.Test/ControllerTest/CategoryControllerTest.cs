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
    public class CategoryControllerTest : BaseControllerTest
    {
        [Fact]
        public async Task GetCategoriesAsync_ReturnCollection()
        {
            /// Arrange
            var service = new Mock<ICategoryService>();
            var categoryList = new List<Category>() {
                FixedData.GetNewCategory(Guid.NewGuid(), "CAT_1"),
                FixedData.GetNewCategory(Guid.NewGuid(), "CAT_2")
            };
            service.Setup(_ => _.GetCategoriesAsync()).Returns(categoryList.AsQueryable());
            var sut = new CategoryController(service.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Category>).ToList().Count.Should().Be(categoryList.Count());
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnEmptyCollection()
        {
            /// Arrange
            var service = new Mock<ICategoryService>();
            service.Setup(_ => _.GetCategoriesAsync()).Returns(new List<Category>().AsQueryable());
            var sut = new CategoryController(service.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Category>).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnFound()
        {
            /// Arrange
            var service = new Mock<ICategoryService>();
            var categoryId = Guid.NewGuid();
            service.Setup(_ => _.GetCategoryByIdAsync(categoryId)).ReturnsAsync(FixedData.GetNewCategory(categoryId, "CAT_1"));
            var sut = new CategoryController(service.Object);

            /// Act
            var result = await sut.Get(categoryId) as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnNotFound()
        {
            /// Arrange
            var service = new Mock<ICategoryService>();
            var invalidId = Guid.NewGuid();
            service.Setup(_ => _.GetCategoryByIdAsync(invalidId)).ReturnsAsync(default(Category));
            var sut = new CategoryController(service.Object);

            /// Act
            var result = await sut.Get(invalidId) as NotFoundResult;

            /// Assert
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task AddCategoryAsync_ReturnSuccess()
        {
            /// Arrange
            var service = new Mock<ICategoryService>();
            var newCategoryId = Guid.NewGuid();
            service.Setup(_ => _.AddCategoryAsync(FixedData.GetNewCategory(newCategoryId, "CAT_NEW"))).ReturnsAsync(FixedData.GetNewCategory(newCategoryId, "CAT_NEW"));
            var sut = new CategoryController(service.Object);

            /// Act
            var result = await sut.Add(FixedData.GetNewCategory(newCategoryId, "CAT_NEW")) as CreatedAtActionResult;

            /// Assert
            result.StatusCode.Should().Be(201);
        }
    }
}
