using Microsoft.AspNetCore.Mvc.Testing;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class WebApplicationFactory<TProgram>
        : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<TProgram>
        where TProgram : class
    {
        public new HttpClient CreateClient()
        {
            return base.CreateClient();
        }
    }
}