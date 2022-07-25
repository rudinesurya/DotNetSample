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
    public class SampleTest : BaseIntegrationTest
    {
        static Func<AppDbContext, bool> seed = (db) =>
        {
            // Seed Products
            db.Products.AddRange(SeedData.IPhoneX, SeedData.S20);

            db.SaveChangesAsync();

            return true;
        };

        public SampleTest() : base("Test", seed) { }

        [Fact]
        public async Task TestForSuccessfullOrderCreation()
        {
            /// Add product to cart and checkout
            /// Verify that the order gets created under the user name


            // Add to cart
            var iPhoneX = SeedData.IPhoneX;
            await TestClient.PostAsync("/cart/additem", FixedData.GetNewAddCartItemAction("Tester", iPhoneX));

            // checkout cart
            var cart = (await TestClient.GetAsync<List<Cart>>("/cart?$filter=UserName eq 'Tester'"))[0];
            await TestClient.PostAsync("/cart/checkout", cart.Id);

            // Verify that the order gets created under the user name
            var result = await TestClient.GetAsync<List<Order>>("/order?$filter=UserName eq 'Tester' &$expand=Items");
            result.Count.Should().Be(1);
            result[0].Items[0].ProductId.Should().Be(iPhoneX.Id);
        }

        [Fact]
        public async Task StressTest1()
        {
            /// Add 2 product to cart and checkout
            /// remove one product and add another to make quantity=2
            /// Verify that the cart contains only one product type with quantity=2
            /// Verify that the order gets created under the user name


            // Add to cart
            var iPhoneX = SeedData.IPhoneX;
            var s20 = SeedData.S20;
            await TestClient.PostAsync("/cart/additem", FixedData.GetNewAddCartItemAction("Tester", iPhoneX));
            await TestClient.PostAsync("/cart/additem", FixedData.GetNewAddCartItemAction("Tester", s20));

            // Get the cart
            var cart = (await TestClient.GetAsync<List<Cart>>("/cart?$filter=UserName eq 'Tester' &$expand=Items($expand=Product)"))[0];

            // Change of decision. Remove iPhoneX
            await TestClient.PostAsync("/cart/removeitem", FixedData.GetNewRemoveCartItemAction(cart.Id, cart.Items.Find(x => x.Product.Id == iPhoneX.Id)));

            // Add another s20
            await TestClient.PostAsync("/cart/additem", FixedData.GetNewAddCartItemAction("Tester", s20));

            // Verify that the cart contains only one product type with quantity=2
            cart = (await TestClient.GetAsync<List<Cart>>("/cart?$filter=UserName eq 'Tester' &$expand=Items"))[0];
            cart.Items.Count.Should().Be(1);
            cart.Items[0].Quantity.Should().Be(2);

            await TestClient.PostAsync("/cart/checkout", cart.Id);

            // Verify that the order gets created under the user name
            var result = await TestClient.GetAsync<List<Order>>("/order?$filter=UserName eq 'Tester' &$expand=Items");
            result.Count.Should().Be(1);
            result[0].Items[0].ProductId.Should().Be(s20.Id);
            result[0].Items[0].Quantity.Should().Be(2);
        }
    }
}
