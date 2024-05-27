using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BriefingService.Models;
using BriefingService.Data;
using Xunit;

public class BriefingsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _context;

    public BriefingsControllerTests(WebApplicationFactory<Program> factory)
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
        _context.Briefings.AddRange(new List<Briefing>
        {
            new Briefing { Name = "Briefing1", Description = "Description1", CreatedBy = "Chewi", CreatedDate = DateTime.Now },
            new Briefing { Name = "Briefing2", Description = "Description2", CreatedBy = "Yoda", CreatedDate = DateTime.Now }
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetBriefings_ReturnsBriefings()
    {
        InitializeDbForTests();

        var response = await _client.GetAsync("/api/Briefings");

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var briefings = JsonSerializer.Deserialize<List<Briefing>>(responseString, _jsonOptions);

        Assert.NotNull(briefings);
        Assert.Equal(2, briefings.Count);
    }

    [Fact]
    public async Task GetBriefing_ReturnsBriefing()
    {
        InitializeDbForTests();

        var response = await _client.GetAsync("/api/Briefings/1");

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var briefing = JsonSerializer.Deserialize<Briefing>(responseString, _jsonOptions);

        Assert.NotNull(briefing);
        Assert.Equal("Briefing1", briefing.Name);
        Assert.Equal("Description1", briefing.Description);
    }

    [Fact]
    public async Task PostBriefing_CreatesBriefing()
    {
        var newBriefing = new Briefing { Name = "Briefing3", Description = "Description3", CreatedBy = "Han Solo", CreatedDate = DateTime.Now };
        var content = new StringContent(JsonSerializer.Serialize(newBriefing, _jsonOptions), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/Briefings", content);

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var createdBriefing = JsonSerializer.Deserialize<Briefing>(responseString, _jsonOptions);

        Assert.NotNull(createdBriefing);
        Assert.Equal("Briefing3", createdBriefing.Name);
        Assert.Equal("Description3", createdBriefing.Description);
        Assert.Equal("Han Solo", createdBriefing.CreatedBy);
    }

    [Fact]
    public async Task PutBriefing_UpdatesBriefing()
    {
        InitializeDbForTests();

        var updatedBriefing = new Briefing { Id = 1, Name = "Briefing1-Updated", Description = "Description1-Updated", CreatedBy = "User1", CreatedDate = DateTime.Now };
        var content = new StringContent(JsonSerializer.Serialize(updatedBriefing, _jsonOptions), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/Briefings/1", content);

        response.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync("/api/Briefings/1");
        var responseString = await getResponse.Content.ReadAsStringAsync();
        var briefing = JsonSerializer.Deserialize<Briefing>(responseString, _jsonOptions);

        Assert.NotNull(briefing);
        Assert.Equal("Briefing1-Updated", briefing.Name);
        Assert.Equal("Description1-Updated", briefing.Description);
    }

    [Fact]
    public async Task DeleteBriefing_DeletesBriefing()
    {
        InitializeDbForTests();

        var response = await _client.DeleteAsync("/api/Briefings/1");

        response.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync("/api/Briefings/1");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
