using FamilyHubs.ReferralUi.Ui.Extensions;
using FamilyHubs.ReferralUi.Ui.Models;
using MassTransit;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddClientServices()
    .AddWebUIServices(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();

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

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";

        options.ClientId = "referral";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.Scope.Add("servicedirectoryapi");
        options.Scope.Add("referralapi");

        options.GetClaimsFromUserInfoEndpoint = true;
    });

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

//app.MapRazorPages();
app.MapRazorPages().RequireAuthorization();

Program.ServiceProvider = app.Services;

app.Run();

public partial class Program
{
    public static IServiceProvider ServiceProvider { get; set; } = default!;
}


