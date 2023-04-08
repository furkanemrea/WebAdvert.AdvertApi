using AdvertApi.HealthChecks;
using AdvertApi.Services.Abstract;
using AdvertApi.Services.Concrete;
using AdvertApi.Services.Concrete.Configuration;
using Amazon.DynamoDBv2;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IAdvertStorageService, DynamoDBAdvertStorageService>();

builder.Services.AddScoped<AmazonDynamoDBClient>();

builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(AdvertProfile)));

builder.Services.AddHealthChecks().AddCheck<StorageHealthCheck>("Storage");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
