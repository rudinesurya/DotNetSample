using DotNetSample.Controllers.Cart_Action;
using DotNetSample.Controllers.Service;
using DotNetSample.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace DotNetSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ODataController
    {
        private readonly ICartService CartService;
        private readonly IOrderService OrderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            CartService = cartService;
            OrderService = orderService;
        }

        [HttpGet(Name = "GetCarts")]
        [EnableQuery]
        [ProducesResponseType(typeof(IList<Cart>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            return Ok(CartService.GetCartsAsync());
        }

        [HttpGet("{id}", Name = "GetCartById")]
        [EnableQuery]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var cart = await CartService.GetCartByIdAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpPost("AddItem", Name = "AddCartItem")]
        [ProducesResponseType(typeof(AddCartItem), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddItem([FromBody] AddCartItem addCartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cart = await CartService.AddItemAsync(addCartItem.UserName, addCartItem.ProductId, addCartItem.Quantity);

            return CreatedAtAction("AddItem", new { id = cart.Id }, addCartItem);
        }

        [HttpPost("RemoveItem", Name = "RemoveCartItem")]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveCartItem removeCartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await CartService.RemoveItemAsync(removeCartItem.CartId, removeCartItem.CartItemId);

            return NoContent();
        }

        [HttpPost("ClearCart", Name = "ClearCart")]
        public async Task<IActionResult> ClearCart([FromBody] Guid cartId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await CartService.ClearCartAsync(cartId);

            return NoContent();
        }

        [HttpPost("Checkout", Name = "CheckoutCart")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        public async Task<IActionResult> CheckoutCart([FromBody] Guid cartId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await OrderService.GetOrderByCartIdAsync(cartId);
            if (order == null)
            {
                var cart = await CartService.GetCartByIdAsync(cartId);

                var newOrder = new Order()
                {
                    Id = Guid.NewGuid(),
                    CartId = cartId,
                    Items = cart.Items.Select(x => OrderItem.CreateFrom(x)).ToList(),
                    UserName = cart.UserName,
                    TotalPrice = cart.TotalPrice,
                    FirstName = "",
                    LastName = "",
                    EmailAddress = "",
                    AddressLine = "",
                    Country = "",
                    State = "",
                    ZipCode = "",
                    CardName = "",
                    CardNumber = "",
                    Expiration = "",
                    CVV = "",
                    PaymentMethod = Entity.PaymentMethod.Paypal,
                };
                order = await OrderService.AddOrderAsync(newOrder);
            }

            return CreatedAtAction("CheckoutCart", new { id = order.Id }, order);
        }
    }
}