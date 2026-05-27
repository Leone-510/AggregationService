using Aggregation.WebAPI;
using Aggregation.WebAPI.Modules.Products;
using Aggregation.WebAPI.Modules.Weather;
using Aggregation.WebAPI.Statistics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMemoryCache();

#region External API Services
builder.Services.Configure<ExternalApiOptions>(builder.Configuration.GetSection("ExternalApis"));

string productsUrl = builder.Configuration["ExternalApis:ProductsApiUrl"] 
    ?? throw new InvalidOperationException("Products API URL is missing in config.");
builder.Services.AddHttpClient<IProductsService, ProductsService>(client =>
{
    client.BaseAddress = new Uri(productsUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(10);
}).AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
});

string weatherUrl = builder.Configuration["ExternalApis:WeatherApiUrl"] 
    ?? throw new InvalidOperationException("Weather API URL is missing in config.");
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri(weatherUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(10);
}).AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
});
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IStatisticsService, StatisticsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
