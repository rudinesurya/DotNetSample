using DotNetSample.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DotNetSample.Test.Helper
{
    public class TestApp : WebApplicationFactory<Program>
    {
        private readonly string Environment;
        private readonly Func<AppDbContext, bool> Seed;

        public TestApp(string environment, Func<AppDbContext, bool> seed)
        {
            Environment = environment;
            Seed = seed;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(Environment);

            var databaseName = "testdb" + Guid.NewGuid().ToString();

            // Add mock/test services to the builder here
            builder.ConfigureServices(services =>
            {
                services.AddScoped(sp =>
                {
                    return new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName)
                        .UseApplicationServiceProvider(sp)
                        .Options;
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    if (db.Database.EnsureCreated())
                    {
                        if (Seed != null)
                            Seed(db);
                    }
                }
            });

            return base.CreateHost(builder);
        }
    }
}
