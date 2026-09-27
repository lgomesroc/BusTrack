using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BusTrack.BusTrack.Tests.UsabilityTests
{
public class UsabilityTests : IDisposable
{
private readonly IWebDriver _driver;

    public UsabilityTests()
    {
        var options =
            new ChromeOptions();

        options.BinaryLocation =
            "/usr/bin/google-chrome";

        options.AddArgument(
            "--headless=new");

        options.AddArgument(
            "--no-sandbox");

        options.AddArgument(
            "--disable-dev-shm-usage");

        options.AddArgument(
            "--window-size=1920,1080");

        _driver =
            new ChromeDriver(options);

        _driver.Manage()
            .Timeouts()
            .ImplicitWait =
                TimeSpan.FromSeconds(5);
    }

    [Fact]
    public void AddPassengerPageTest()
    {
        _driver.Navigate()
            .GoToUrl(
                "http://localhost:4200/enter-the-system");

        var emailField =
            _driver.FindElement(
                By.Id("email"));

        var passwordField =
            _driver.FindElement(
                By.Id("password"));

        var loginButton =
            _driver.FindElement(
                By.CssSelector(
                    "button[type='submit']"));

        emailField.SendKeys(
            Environment.GetEnvironmentVariable(
                "BUSTRACK_TEST_EMAIL")
            ?? throw new InvalidOperationException(
                "A variável de ambiente BUSTRACK_TEST_EMAIL não foi definida."));

        passwordField.SendKeys(
            Environment.GetEnvironmentVariable(
                "BUSTRACK_TEST_PASSWORD")
            ?? throw new InvalidOperationException(
                "A variável de ambiente BUSTRACK_TEST_PASSWORD não foi definida."));

        loginButton.Click();

        AcceptAlertIfPresent();

        WaitForUrl(
            "/dashboard");

        var passengersCard =
            _driver.FindElement(
                By.XPath(
                    "//button[contains(@class,'dashboard-card')]" +
                    "[.//h2[normalize-space()='Passageiros']]"));

        Assert.NotNull(
            passengersCard);

        passengersCard.Click();

        WaitForUrl(
            "/passengers");

        Assert.EndsWith(
            "/passengers",
            _driver.Url);
    }

    private void AcceptAlertIfPresent()
    {
        var timeout =
            DateTime.UtcNow.AddSeconds(5);

        while (
            DateTime.UtcNow < timeout)
        {
            try
            {
                var alert =
                    _driver.SwitchTo().Alert();

                alert.Accept();

                return;
            }
            catch (NoAlertPresentException)
            {
                Thread.Sleep(250);
            }
        }
    }

    private void WaitForUrl(
        string expectedPath)
    {
        var timeout =
            DateTime.UtcNow.AddSeconds(10);

        while (
            DateTime.UtcNow < timeout)
        {
            try
            {
                if (_driver.Url.EndsWith(
                    expectedPath,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
            catch (UnhandledAlertException)
            {
                AcceptAlertIfPresent();
            }

            Thread.Sleep(250);
        }

        throw new Xunit.Sdk.XunitException(
            $"A URL esperada '{expectedPath}' não foi alcançada. " +
            $"URL atual: '{_driver.Url}'.");
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}

}
