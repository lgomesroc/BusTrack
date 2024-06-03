using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class WebApplicationFactory<TProgram> where TProgram : class
    {
        protected virtual void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TProgram>(); 
        }

        public HttpClient CreateClient()
        {
            var builder = new WebHostBuilder();
            ConfigureWebHost(builder);
            var server = new TestServer(builder);
            return server.CreateClient();
        }
    }
}
