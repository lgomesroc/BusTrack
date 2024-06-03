using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace BusTrack.BusTrack.Tests.UsabilityTests
{
    public class UsabilityTests : IDisposable
    {
        private readonly IWebDriver _driver;

        public UsabilityTests()
        {
            _driver = new ChromeDriver();
        }

        [Fact]
        public void AddPassengerPageTest()
        {
            _driver.Navigate().GoToUrl("http://localhost:5000/add-passenger");
            var nameField = _driver.FindElement(By.Name("name"));
            var emailField = _driver.FindElement(By.Name("email"));
            var ageField = _driver.FindElement(By.Name("age"));
            var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

            nameField.SendKeys("Test User");
            emailField.SendKeys("test.user@example.com");
            ageField.SendKeys("25");
            submitButton.Click();

            var successMessage = _driver.FindElement(By.Id("success-message"));
            Assert.NotNull(successMessage);
            Assert.Equal("Passenger added successfully!", successMessage.Text);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
