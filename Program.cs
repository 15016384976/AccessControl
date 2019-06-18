using AccessControl.Architects.Entities;
using AccessControl.Architects.Entities.Contexts;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessControl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope()) Initialize(scope.ServiceProvider);
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void Initialize(IServiceProvider provider)
        {
            var configurationDbContext = provider.GetRequiredService<ConfigurationDbContext>();

            if (configurationDbContext.IdentityResources.Any() == false)
            {
                new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile()
                }
                .ForEach(v =>
                {
                    configurationDbContext.IdentityResources.Add(v.ToEntity());
                });
                configurationDbContext.SaveChanges();
            }

            if (configurationDbContext.ApiResources.Any() == false)
            {
                new List<ApiResource>
                {
                    new ApiResource("api", "api display name")
                }.ForEach(v =>
                {
                    configurationDbContext.ApiResources.Add(v.ToEntity());
                });
                configurationDbContext.SaveChanges();
            }

            if (configurationDbContext.Clients.Any() == false)
            {
                new List<Client>
                {
                    new Client
                    {
                        ClientId = "client",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedScopes = { "api" }
                    },
                    new Client
                    {
                        ClientId = "js",
                        ClientName = "JavaScript Client",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequirePkce = true,
                        RequireClientSecret = false,
                        RequireConsent = false,
                        RedirectUris = { "http://localhost:8080/transfer" },
                        PostLogoutRedirectUris = { "http://localhost:8080/" },
                        AllowedScopes = { "openid", "profile", "api" },
                        AccessTokenLifetime = 120
                    }
                }
                .ForEach(v =>
                {
                    configurationDbContext.Clients.Add(v.ToEntity());
                });
                configurationDbContext.SaveChanges();
            }

            var databaseContext = provider.GetRequiredService<DatabaseContext>();
            
            /*
            databaseContext.RoleEntities.AddRange(new RoleEntity[]
            {
                new RoleEntity
                {
                    Id = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    Name = "Operation",
                    Available = true,
                    Description = "Operation description"
                },
                new RoleEntity
                {
                    Id = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    Name = "Program",
                    Available = true,
                    Description = "Program description"
                },
                new RoleEntity
                {
                    Id = Guid.Parse("d2dcdcdb-a249-4635-b8b7-8fd6a502a681"),
                    Name = "Business",
                    Available = true,
                    Description = "Business description"
                }
            });
            databaseContext.SaveChanges();
            */

            /*
            databaseContext.UserEntities.AddRange(new UserEntity[]
            {
                new UserEntity
                {
                    Id = Guid.Parse("161ea09a-db26-4bd3-9e24-b6556ada85b4"),
                    Account = "admin",
                    Password = "admin",
                    Available = true,
                    Description = ""
                }
            });
            databaseContext.SaveChanges();
            */

            /*
            databaseContext.RoleUserEntities.AddRange(new RoleUserEntity[]
            {
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    UserId = Guid.Parse("")
                },

                new RoleUserEntity
                {
                    RoleId = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    UserId = Guid.Parse("")
                },

                new RoleUserEntity
                {
                    RoleId = Guid.Parse("d2dcdcdb-a249-4635-b8b7-8fd6a502a681"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("d2dcdcdb-a249-4635-b8b7-8fd6a502a681"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("d2dcdcdb-a249-4635-b8b7-8fd6a502a681"),
                    UserId = Guid.Parse("")
                },

                new RoleUserEntity
                {
                    RoleId = Guid.Parse("0703a0c8-0367-4207-97ec-0249bc5c30db"),
                    UserId = Guid.Parse("")
                },
                new RoleUserEntity
                {
                    RoleId = Guid.Parse("0703a0c8-0367-4207-97ec-0249bc5c30db"),
                    UserId = Guid.Parse("")
                },

                new RoleUserEntity
                {
                    RoleId = Guid.Parse("164ba1b7-cedc-4398-9458-095a6f2d208e"),
                    UserId = Guid.Parse("")
                }
            });
            */

            /*
            databaseContext.PermissionEntities.AddRange(new PermissionEntity[]
            {
                new PermissionEntity
                {
                    Id = Guid.Parse("400fead9-583d-4c4f-aeb9-18fc22be2a9a"),
                    ParentId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Type = 1,
                    Code = "System",
                    Name = "System management",
                    Icon = "icon-system",
                    Available = true,
                    Description = "System management description"
                },
                new PermissionEntity
                {
                    Id = Guid.Parse("785a74aa-ce86-4c3a-887d-c40249e2384a"),
                    ParentId = Guid.Parse("400fead9-583d-4c4f-aeb9-18fc22be2a9a"),
                    Type = 1,
                    Code = "Permission",
                    Name = "Permission management",
                    Icon = "icon-permission",
                    Available = true,
                    Description = "Permission management description"
                },
                new PermissionEntity
                {
                    Id = Guid.Parse("a233bf01-0446-4d5f-8c81-e5393bf4b1f3"),
                    ParentId = Guid.Parse("400fead9-583d-4c4f-aeb9-18fc22be2a9a"),
                    Type = 1,
                    Code = "Role",
                    Name = "Role management",
                    Icon = "icon-role",
                    Available = true,
                    Description = "Role management description"
                },
                new PermissionEntity
                {
                    Id = Guid.Parse("8865bf60-e8aa-46b8-abe4-1c6ae128630c"),
                    ParentId = Guid.Parse("400fead9-583d-4c4f-aeb9-18fc22be2a9a"),
                    Type = 1,
                    Code = "User",
                    Name = "User management",
                    Icon = "icon-user",
                    Available = true,
                    Description = "User management description"
                }
            });
            */
            /*
            databaseContext.RolePermissionEntities.AddRange(new RolePermissionEntity[]
            {
                new RolePermissionEntity
                {
                    RoleId = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    PermissionId = Guid.Parse("400fead9-583d-4c4f-aeb9-18fc22be2a9a")
                },
                new RolePermissionEntity
                {
                    RoleId = Guid.Parse("e730c5a2-2bbd-43f6-90e5-9abfb06ffc24"),
                    PermissionId = Guid.Parse("785a74aa-ce86-4c3a-887d-c40249e2384a")
                },
                new RolePermissionEntity
                {
                    RoleId = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    PermissionId = Guid.Parse("400fead9-583d-4c4f-aeb9-18fc22be2a9a")
                },
                new RolePermissionEntity
                {
                    RoleId = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    PermissionId = Guid.Parse("a233bf01-0446-4d5f-8c81-e5393bf4b1f3")
                },
                new RolePermissionEntity
                {
                    RoleId = Guid.Parse("04764cb6-2ccc-4ecc-bc9c-133b28c652ea"),
                    PermissionId = Guid.Parse("8865bf60-e8aa-46b8-abe4-1c6ae128630c")
                }
            });
            */
        }
    }
}
