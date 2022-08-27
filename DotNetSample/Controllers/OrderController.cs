using DotNetSample.Controllers.Service;
using DotNetSample.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace DotNetSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ODataController
    {
        private readonly IOrderService OrderService;

        public OrderController(IOrderService orderService)
        {
            OrderService = orderService;
        }

        [HttpGet(Name = "GetOrders")]
        [EnableQuery]
        [ProducesResponseType(typeof(IList<Order>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            return Ok(OrderService.GetOrdersAsync());
        }

        [HttpGet("id/{id}", Name = "GetOrderById")]
        [EnableQuery]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var order = await OrderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}