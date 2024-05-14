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

var builder = WebApplication.CreateBuilder(args);

// BsonSeralizer... fortæller at hver gang den ser en Guid i alle entiteter skal den serializeres til en string. 
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

// OBS: lig dem her op i vault, se opgave
string mySecret = Environment.GetEnvironmentVariable("Secret") ?? "none";
string myIssuer = Environment.GetEnvironmentVariable("Issuer") ?? "none";

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
// Add services to the container.

// miljøvariabler ign terminal
builder.Services.Configure<VaultSettings>(options =>
{
    options.Address = Environment.GetEnvironmentVariable("Address");
    options.Token = Environment.GetEnvironmentVariable("Token");
});

//tilføjer Repository til services
// builder.Services.AddTransient<IMongoDBRepository, MongoDBLoginRepository>();
builder.Services.AddTransient<IVaultService, VaultService>();
// Konfigurer HttpClient for AuctionHouse
var useraddress = Environment.GetEnvironmentVariable("UserAddress");
builder.Services.AddHttpClient<IUserRepository, UserRespository>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5265"); // URL til UserService

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
