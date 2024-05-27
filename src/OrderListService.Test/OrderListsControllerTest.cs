using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderListService.Models;
using OrderListService.Data;
using Xunit;

public class OrderListsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _context;

    public OrderListsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        });

        _client = _factory.CreateClient();

        // Match JSON options with those used in the main program
        _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        var serviceProvider = _factory.Services;
        _scope = serviceProvider.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _scope.Dispose();
    }

    private void InitializeDbForTests()
    {
        _context.OrderLists.AddRange(new List<OrderList>
        {
            new OrderList { OrderNumber = "ORD1234", CustomerName = "I AM GROOT" },
            new OrderList { OrderNumber = "Order002", CustomerName = "Tony Stark" }
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetOrderLists_ReturnsOrderLists()
    {
        InitializeDbForTests();

        var response = await _client.GetAsync("/api/OrderLists");

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var orderLists = JsonSerializer.Deserialize<List<OrderList>>(responseString, _jsonOptions);

        Assert.NotNull(orderLists);
        Assert.Equal(2, orderLists.Count);
    }

    [Fact]
    public async Task GetOrderList_ReturnsOrderList()
    {
        InitializeDbForTests();

        var response = await _client.GetAsync("/api/OrderLists/1");

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var orderList = JsonSerializer.Deserialize<OrderList>(responseString, _jsonOptions);

        Assert.NotNull(orderList);
        Assert.Equal("ORD1234", orderList.OrderNumber);
        Assert.Equal("I AM GROOT", orderList.CustomerName);
    }

    [Fact]
    public async Task PostOrderList_CreatesOrderList()
    {
        var newOrderList = new OrderList { OrderNumber = "ORD345", CustomerName = "Star Lord", OrderDate = new DateTime(2023, 5, 23) };
        var content = new StringContent(JsonSerializer.Serialize(newOrderList, _jsonOptions), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/OrderLists", content);

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var createdOrderList = JsonSerializer.Deserialize<OrderList>(responseString, _jsonOptions);

        Assert.NotNull(createdOrderList);
        Assert.Equal("ORD345", createdOrderList.OrderNumber);
        Assert.Equal("Star Lord", createdOrderList.CustomerName);
        Assert.Equal(new DateTime(2023, 5, 23), createdOrderList.OrderDate);
    }

    [Fact]
    public async Task PutOrderList_UpdatesOrderList()
    {
        InitializeDbForTests();

        var updatedOrderList = new OrderList { Id = 1, OrderNumber = "ORD3000", CustomerName = "I AM GROOT! - with an update", OrderDate = new DateTime(2023, 5, 24) };
        var content = new StringContent(JsonSerializer.Serialize(updatedOrderList, _jsonOptions), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/OrderLists/1", content);

        response.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync("/api/OrderLists/1");
        var responseString = await getResponse.Content.ReadAsStringAsync();
        var orderList = JsonSerializer.Deserialize<OrderList>(responseString, _jsonOptions);

        Assert.NotNull(orderList);
        Assert.Equal("ORD3000", orderList.OrderNumber);
        Assert.Equal("I AM GROOT! - with an update", orderList.CustomerName);
        Assert.Equal(new DateTime(2023, 5, 24), orderList.OrderDate);
    }

    [Fact]
    public async Task DeleteOrderList_DeletesOrderList()
    {
        InitializeDbForTests();

        var response = await _client.DeleteAsync("/api/OrderLists/1");

        response.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync("/api/OrderLists/1");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
