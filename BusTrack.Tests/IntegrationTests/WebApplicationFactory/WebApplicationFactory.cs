namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class WebApplicationFactory<TProgram>
        : CustomWebApplicationFactory<TProgram>
        where TProgram : class
    {
        public new HttpClient CreateClient()
        {
            return base.CreateClient();
        }
    }
}
