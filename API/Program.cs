using API.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretkey"));
var keyBytes = key.Key;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthConstants.ChatRoomCreatorPolicy, policy => {
        policy.Requirements.Add(new ChatRoomCreatorRequirement());
    });
});

builder.Services.AddTransient<IAuthorizationHandler, ChatRoomCreatorAuthorizationHandler>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
