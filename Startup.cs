using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessControl.Architects.Components.IdentityServer;
using AccessControl.Architects.Entities.Contexts;
using AccessControl.Architects.Services;
using AccessControl.Architects.Services.Interfaces;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IClaimService, ClaimService>();

            services.AddSingleton<IPermissionService, PermissionService>();

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AccessControl;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            });

            services.AddScoped<IUserService, UserService>();

            services.AddMvc(options =>
                    {
                        options.Filters.Add(new ExceptionFilter());
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddIdentityServer(options =>
                    {
                        options.Authentication.CookieAuthenticationScheme = "Cookies";
                    })
                    .AddDeveloperSigningCredential()
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseInMemoryDatabase("AccessControl");
                        };
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseInMemoryDatabase("AccessControl");
                        };
                        options.EnableTokenCleanup = true;
                    })
                    .AddProfileService<ProfileService>()
                    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            services.AddAuthentication("Bearer")
                    .AddJwtBearer(options =>
                    {
                        options.Authority = Configuration["urls"];
                        options.RequireHttpsMetadata = false;
                        options.Audience = "api";
                    })
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Account/Login";
                    });

            services.AddCors(options =>
            {
                options.AddPolicy("default", builder =>
                {
                    builder.WithOrigins("http://localhost:8080")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseCors("default");
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }

    public interface IClaimService
    {
        string SubjectId { get; }
    }

    public class ClaimService : IClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string SubjectId => _httpContextAccessor.HttpContext?.User.Claims.SingleOrDefault(v => v.Type == "sub")?.Value;
    }

    public interface IPermissionService
    {
        Task<bool> ValidateAsync(string identifierValue, string permissionName);
    }

    public class PermissionService : IPermissionService
    {
        public Task<bool> ValidateAsync(string identifierValue, string permissionName)
        {
            //var member = _members.FirstOrDefault(v => v.Id == memberId);
            //if (member == null) return false;
            //return member.Admin || member.Permissions.Any(v => permissionName.StartsWith(v.PermissionName));
            return Task.FromResult(false);
        }
    }

    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }

        public PermissionAuthorizationRequirement(string permissionName)
        {
            PermissionName = permissionName;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            if (context.User != null)
            {
                var identifier = context.User.FindFirst(v => v.Type == ClaimTypes.NameIdentifier);
                if (identifier != null)
                {
                    if (await _permissionService.ValidateAsync(identifier.Value, requirement.PermissionName))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string PermissionName { get; }

        public PermissionAttribute(string permissionName)
        {
            PermissionName = permissionName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var service = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var result = await service.AuthorizeAsync(context.HttpContext.User, null, new PermissionAuthorizationRequirement(PermissionName));
            if (result.Succeeded == false)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            if (context.Exception is ValidationException)
            {
                statusCode = StatusCodes.Status400BadRequest;
            }
            context.Result = new ObjectResult(context.Exception.Message) { StatusCode = statusCode };
        }
    }

    public interface IEntity
    {

    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {

        }
    }
}

// Install-Package MediatR
// Install-Package MediatR.Extensions.Microsoft.DependencyInjection
// Install-Package IdentityServer4
// Install-Package IdentityServer4.AccessTokenValidation
// Install-Package IdentityServer4.EntityFramework
// Install-Package Dapper
