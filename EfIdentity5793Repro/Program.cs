using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace EfIdentity5793Repro
{

    class Program
    {
        static void Main(string[] args)
        {
            var sp = BuildServiceProvider();

            // Ensure database created..
            using (var scope = sp.CreateScope())
            {
                var scopedSp = scope.ServiceProvider;
                var dbContext = scopedSp.GetRequiredService<ReproDbContext>();

                dbContext.Database.EnsureCreated();
            }

            // Repro issue.
            using (var scope = sp.CreateScope())
            {
                var scopedSp = scope.ServiceProvider;
                var userManager = scopedSp.GetRequiredService<UserManager<User>>();

                var newUserName = Guid.NewGuid().ToString();
                var newUser = new User() { Email = $"{newUserName}@foo.com", UserName = newUserName };
                var result = userManager.CreateAsync(newUser, "Passw0rd#1").Result;

                var blowsUp = userManager.GetClaimsAsync(newUser).Result;

            }

        }
        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            // UserManager has a hidden dependencies on these services being added first:
            services.AddLogging();
            services.AddDataProtection();

            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ReproDbContext>((o) =>
            {
                o.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=repro;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            });


            AddIdentityServices<User, Role>(services)
                        .AddEntityFrameworkStores<ReproDbContext>()
                       .AddDefaultTokenProviders();
        }

        public static IdentityBuilder AddIdentityServices<TUser, TRole>(IServiceCollection services)
where TUser : class
where TRole : class
        {
            // Identity services
            services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
            services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.TryAddScoped<IRoleValidator<TRole>, RoleValidator<TRole>>();
            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
            services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<TUser>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();
            services.TryAddScoped<UserManager<TUser>>();
            services.TryAddScoped<SignInManager<TUser>>();
            services.TryAddScoped<RoleManager<TRole>>();

            return new IdentityBuilder(typeof(TUser), typeof(TRole), services);
        }

    }
}
