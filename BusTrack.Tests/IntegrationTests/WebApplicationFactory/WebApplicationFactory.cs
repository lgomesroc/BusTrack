using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class WebApplicationFactory<TProgram>
        where TProgram : class
    {
        protected virtual void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseStartup<TProgram>();
        }

        public HttpClient CreateClient()
        {
            var host =
                new HostBuilder()
                    .ConfigureWebHostDefaults(
                        builder =>
                        {
                            ConfigureWebHost(builder);
                            builder.UseTestServer();
                        })
                    .Start();

            return host.GetTestClient();
        }
    }
}
