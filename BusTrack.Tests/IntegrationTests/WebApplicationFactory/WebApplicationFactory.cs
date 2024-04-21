using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class WebApplicationFactory<TProgram> where TProgram : class
    {
        protected virtual void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TProgram>(); // Configura o servidor web para usar o Program.cs como a inicialização da aplicação
        }

        public HttpClient CreateClient()
        {
            // Cria um cliente HTTP para interagir com a aplicação hospedada
            var builder = new WebHostBuilder();
            ConfigureWebHost(builder);
            var server = new TestServer(builder);
            return server.CreateClient();
        }
    }
}
