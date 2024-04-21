using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Carrega as configurações do ambiente de teste
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            // Configura o servidor web para usar o programa principal do BusTrack
            builder.UseStartup<TProgram>();

            // Adiciona serviços específicos para o ambiente de teste (se necessário)
            builder.ConfigureServices(services =>
            {
                // Configuração de serviços de teste
                // services.AddScoped<ISomeService, MockSomeService>();
            });

            // Ajusta o comportamento do host para o ambiente de teste
            builder.UseEnvironment("Testing");
        }

        public HttpClient CreateClient()
        {
            // Cria um cliente HTTP para interagir com a aplicação hospedada
            return CreateClient();
        }
    }
}

