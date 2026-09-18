using Microsoft.AspNetCore.Hosting;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }

        public new HttpClient CreateClient()
        {
            return base.CreateClient();
        }
    }
}
