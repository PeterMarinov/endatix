using Endatix.Framework.Hosting;
using Endatix.Api.Setup;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Endatix.Api.Infrastructure;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Endatix.Infrastructure.Identity;
using Microsoft.Extensions.Hosting;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Endatix.Setup;

/// <summary>
/// Provides extension methods for configuring and setting up the Endatix application
/// </summary>
public static class EndatixAppExtensions
{
    private const int JWT_CLOCK_SKEW_IN_SECONDS = 15;

    /// <summary>
    /// Adds the API Endpoints associated provided by the Endatix app
    /// </summary>
    /// <param name="endatixApp"></param>
    /// <returns>An instance of <see cref="IEndatixApp"/> representing the configured application.</returns>
    public static IEndatixApp AddApiEndpoints(this IEndatixApp endatixApp)
    {
        Guard.Against.Null(endatixApp?.WebHostBuilder?.Configuration);
        var jwtSettings = endatixApp.WebHostBuilder.Configuration
                         .GetRequiredSection(JwtOptions.SECTION_NAME)
                         .Get<JwtOptions>();
        Guard.Against.Null(jwtSettings);

        var isDevelopment = endatixApp.WebHostBuilder.Environment.IsDevelopment();
        endatixApp.Services.AddAuthenticationJwtBearer(
                   signingOptions => signingOptions.SigningKey = jwtSettings.SigningKey,
                   bearerOptions =>
                   {
                       bearerOptions.RequireHttpsMetadata = isDevelopment ? false : true;
                       bearerOptions.TokenValidationParameters = new TokenValidationParameters
                       {
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                           ValidIssuer = jwtSettings.Issuer,
                           ValidAudiences = jwtSettings.Audiences,
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ValidateIssuerSigningKey = true,
                           ClockSkew = TimeSpan.FromSeconds(JWT_CLOCK_SKEW_IN_SECONDS)
                       };
                   });

        endatixApp.Services.AddAuthorization();
        endatixApp.Services.AddCorsServices();
        endatixApp.Services.AddDefaultJsonOptions();

        endatixApp.Services
                .AddFastEndpoints()
                .SwaggerDocument(o =>
                    {
                        o.ShortSchemaNames = true;
                        o.DocumentSettings = s =>
                        {
                            s.Version = "v0.1.0";
                            s.DocumentName = "Alpha Release";
                            s.Title = "Endatix API";
                        };
                    });

        return endatixApp;
    }

    /// <summary>
    /// Adds the default JSON options Endatix needs for minimal API results.
    /// </summary>
    private static IServiceCollection AddDefaultJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new LongToStringConverter()));

        return services;
    }
}
