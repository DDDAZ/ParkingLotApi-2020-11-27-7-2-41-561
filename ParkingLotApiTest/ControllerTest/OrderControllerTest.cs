﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ParkingLotApi;
using ParkingLotApi.Dtos;
using ParkingLotApi.Repository;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ParkingLotApiTest.ControllerTest
{
    [Collection("ParkingLotTest")]
    public class OrderControllerTest : TestBase
    {
        private readonly ParkingLotContext context;
        private readonly HttpClient client;
        public OrderControllerTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            var scope = Factory.Services.CreateScope();
            var scopeService = scope.ServiceProvider;
            context = scopeService.GetRequiredService<ParkingLotContext>();
            client = GetClient();
        }

        [Fact]
        public async Task Story2_AC1_3_Should_add_order_correctly()
        {
            // given
            var parkingLot = new ParkingLot("Lot1", 2, "location1");
            var order1 = new OrderRequest("Lot1", "JA00001");
            var order2 = new OrderRequest("Lot1", "JA00002");
            var order3 = new OrderRequest("Lot1", "JA00003");

            // when
            await client.PostAsync("/parkinglots", GetRequestContent(parkingLot));
            var response = await client.PostAsync("/orders", GetRequestContent(order1));
            var orderReturn = await GetResponseContent<Order>(response);

            var responseOrderNotClosed = await client.PostAsync("/orders", GetRequestContent(order1));

            await client.PostAsync("/orders", GetRequestContent(order2));
            var responseFullLot = await client.PostAsync("/orders", GetRequestContent(order3));

            // then
            Assert.Equal(orderReturn, new Order(context.Orders.FirstOrDefaultAsync().Result));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Contains("/orders/1", response.Headers.Location.AbsoluteUri);

            Assert.Equal(HttpStatusCode.BadRequest, responseOrderNotClosed.StatusCode);

            Assert.Equal(HttpStatusCode.BadRequest, responseFullLot.StatusCode);
        }

        private async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<T>(body);

            return content;
        }

        private StringContent GetRequestContent<T>(T requestBody)
        {
            var httpContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(httpContent, Encoding.UTF8, MediaTypeNames.Application.Json);

            return content;
        }
    }
}