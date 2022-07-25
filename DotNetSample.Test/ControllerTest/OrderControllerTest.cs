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
    public class OrderControllerTest : BaseControllerTest
    {
        [Fact]
        public async Task GetOrdersAsync_ReturnCollection()
        {
            /// Arrange
            var service = new Mock<IOrderService>();
            var orderList = new List<Order>() {
                FixedData.GetNewOrder(Guid.NewGuid(), FixedData.GetNewCart(Guid.NewGuid(), "USER_1"))
            };
            service.Setup(_ => _.GetOrdersAsync()).Returns(orderList.AsQueryable());
            var sut = new OrderController(service.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Order>).ToList().Count.Should().Be(orderList.Count());
        }

        [Fact]
        public async Task GetOrdersAsync_ReturnEmptyCollection()
        {
            /// Arrange
            var service = new Mock<IOrderService>();
            service.Setup(_ => _.GetOrdersAsync()).Returns(new List<Order>().AsQueryable());
            var sut = new OrderController(service.Object);

            /// Act
            var result = await sut.Get() as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
            (result.Value as IQueryable<Order>).ToList().Count.Should().Be(0);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnFound()
        {
            /// Arrange
            var service = new Mock<IOrderService>();
            var orderId = Guid.NewGuid();
            service.Setup(_ => _.GetOrderByIdAsync(orderId)).ReturnsAsync(FixedData.GetNewOrder(orderId, FixedData.GetNewCart(Guid.NewGuid(), "USER_1")));
            var sut = new OrderController(service.Object);

            /// Act
            var result = await sut.Get(orderId) as OkObjectResult;

            /// Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnNotFound()
        {
            /// Arrange
            var service = new Mock<IOrderService>();
            var invalidId = Guid.NewGuid();
            service.Setup(_ => _.GetOrderByIdAsync(invalidId)).ReturnsAsync(default(Order));
            var sut = new OrderController(service.Object);

            /// Act
            var result = await sut.Get(invalidId) as NotFoundResult;

            /// Assert
            result.StatusCode.Should().Be(404);
        }
    }
}
