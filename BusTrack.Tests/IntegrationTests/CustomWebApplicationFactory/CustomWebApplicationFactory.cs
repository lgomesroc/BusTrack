using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.UseStartup<TProgram>();

            builder.ConfigureServices(services =>
            {
            });

            builder.UseEnvironment("Testing");
        }

        public HttpClient CreateClient()
        {
            return base.CreateClient();
        }
    }
}

