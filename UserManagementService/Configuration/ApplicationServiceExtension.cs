using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Abstraction;
using Repository.Repos;
using Services.Abstraction;
using Services.Services;
using Services.Utility;
using System.Text;

namespace UserManagementService.Configuration
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddRequiredServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(c =>
            {
                c.UseSqlServer(configuration.GetConnectionString("AppDbContext"));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(c =>
                    {
                        c.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration["Jwt:Issuer"],
                            ValidAudience = configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]))

                        };
                    });

            services.AddCors(c => {
                c.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials().Build()
                );
            });
            services.AddApplicationDependency();
            return services;
        }
        
        private static IServiceCollection AddApplicationDependency(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IApplicationContext, ApplicationContext>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
        public static IServiceCollection AddConsulConfig(this IServiceCollection service,IConfiguration configuration)
        {
            var consulAddress = configuration["ConsulUrl"];
            service.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(con => {
                con.Address = new Uri(consulAddress);
            }));
            return service;

        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app,IHost host)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var features = host.Services.GetService<IServer>();
            var addresses = features.Features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.Last();

            var url = new Uri(address);

            var registration = new AgentServiceRegistration()
            {
                ID = "UserManagement-"+url.Port.ToString(),
                Name = "UserManagement",
                Address = $"{url.Host}",
                Port = url.Port
            };
            logger.LogInformation("Registered with consul");
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);
            lifetime.ApplicationStopping.Register(() => {
                logger.LogInformation("deregister");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });
            return app;

        }
    }
}
