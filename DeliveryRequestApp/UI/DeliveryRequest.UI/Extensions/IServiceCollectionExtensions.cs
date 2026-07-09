using DeliveryRequest.UI.Data;
using DeliveryRequest.UI.Http;
using DeliveryRequest.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRequest.UI.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.
        services.AddControllersWithViews();

        services.AddIdentity(configuration);
        services.AddHttpContextAccessor();
        services.AddApis(configuration);

        return services;
    }

    private static IServiceCollection AddApis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<CurrentUserHeaderHandler>();
        services.AddHttpClient("DeliveryRequestApi", client =>
        {
            client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
        })
        .AddHttpMessageHandler<CurrentUserHeaderHandler>();

        services.AddHttpClient<IReportApiClient, ReportApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiSettings:ReportBaseUrl"]!);
        })
        .AddHttpMessageHandler<CurrentUserHeaderHandler>();

        services.AddHttpClient<IAuditApiClient, AuditApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiSettings:AuditBaseUrl"]!);
        })
        .AddHttpMessageHandler<CurrentUserHeaderHandler>();
        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
           options.UseSqlServer(configuration.GetConnectionString("IdentityDb")));

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        // Every action requires an authenticated user by default; use [AllowAnonymous] to opt out
        // (e.g. AccountController's Login/AccessDenied actions).
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
