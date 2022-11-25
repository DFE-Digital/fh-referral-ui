using FamilyHubs.ReferralUi.Ui.Extensions;
using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Extensions;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
RegisterComponents(builder.Services, builder.Configuration);

builder.Services.AddTransient<IRedisCache, RedisCache>();
builder.Services.AddTransient<IRedisCacheService, RedisCacheService>();
builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddTransient<ITokenService, TokenService>();

// Add services to the container.
builder.AddClientServices();

// Add services to the container.
builder.Services
    .AddWebUIServices(builder.Configuration);



// Add services to the container.
builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    //options.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Referrer", policy =>
                    policy.RequireAssertion(context =>
                                context.User.IsInRole("VCSAdmin") ||
                                context.User.IsInRole("Professional")));
});


if (builder.Configuration.GetValue<bool>("UseRabbitMQ"))
{
    var rabbitMqSettings = builder.Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
    builder.Services.AddMassTransit(mt =>
                        mt.UsingRabbitMq((cntxt, cfg) =>
                        {
                            cfg.Host(rabbitMqSettings.Uri, "/", c =>
                            {
                                c.Username(rabbitMqSettings.UserName);
                                c.Password(rabbitMqSettings.Password);
                            });
                        }));
}
var app = builder.Build();



    // Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

Program.ServiceProvider = app.Services;

app.Run();

static void RegisterComponents(IServiceCollection builder, IConfiguration configuration)
{
    builder.AddApplicationInsights(configuration, "fh_referralui_ui");
}
public partial class Program
{
    public static IServiceProvider ServiceProvider { get; set; } = default!;
}


