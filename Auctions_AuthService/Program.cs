using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Repositories;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using Microsoft.Extensions.Options;
using NLog.Web;
using NLog;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Extensions.DependencyInjection;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
.GetCurrentClassLogger();
logger.Debug("init main");


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

// BsonSeralizer... fortæller at hver gang den ser en Guid i alle entiteter skal den serializeres til en string. 
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

// Fetch secrets from Vault. Jeg initierer vaultService og bruger metoden derinde GetSecretAsync
var vaultService = new VaultService(logger, builder.Configuration);
var mySecret = await vaultService.GetSecretAsync("Secret");
var myIssuer = await vaultService.GetSecretAsync("Issuer");
// logger.Info($"Secret: {mySecret} and Issuer: {myIssuer}");
if (mySecret == null || myIssuer == null)
{
    Console.WriteLine("Failed to retrieve secrets from Vault");
    throw new ApplicationException("Failed to retrieve secrets from Vault");
}

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = myIssuer,
        ValidAudience = "http://localhost",
        IssuerSigningKey =
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret))
    };
});
// Tilføjer authorization politikker som bliver brugt i controlleren, virker ik
builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    });
// Add services to the container.

//tilføjer Repository til services.
builder.Services.AddSingleton<IVaultService>(vaultService);

// Konfigurer HttpClient for AuctionService udfra environment variablen UserServiceUrl
var auctionServiceUrl = Environment.GetEnvironmentVariable("auctionServiceUrl");
if (string.IsNullOrEmpty(auctionServiceUrl))
{
    logger.Error("AuctionServiceUrl is missing");
    throw new ApplicationException("AuctionServiceUrl is missing");
}
else
{
    logger.Info("UserServiceUrl is: " + auctionServiceUrl);
}
builder.Services.AddHttpClient<IUserRepository, UserRespository>(client =>
{
    client.BaseAddress = new Uri(auctionServiceUrl);
});


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
