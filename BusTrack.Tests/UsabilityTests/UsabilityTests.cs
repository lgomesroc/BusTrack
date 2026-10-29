using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BusTrack.BusTrack.Tests.UsabilityTests
{
    public class UsabilityTests : IDisposable
    {
        private readonly IWebDriver _driver;

        public UsabilityTests()
        {
            var options = new ChromeOptions();

            var chromeBinary = ResolveChromeBinary();

            options.BinaryLocation = chromeBinary;

            options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1920,1080");

            if (OperatingSystem.IsLinux())
            {
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
            }

            _driver = new ChromeDriver(options);

            _driver.Manage()
                .Timeouts()
                .ImplicitWait = TimeSpan.FromSeconds(5);
        }

        private static string ResolveChromeBinary()
        {
            var configuredPath =
                Environment.GetEnvironmentVariable("CHROME_BIN");

            if (!string.IsNullOrWhiteSpace(configuredPath))
            {
                if (File.Exists(configuredPath))
                {
                    return configuredPath;
                }

                throw new FileNotFoundException(
                    "A variável CHROME_BIN está definida, mas o arquivo " +
                    $"não existe: '{configuredPath}'.");
            }

            var candidates = new List<string>();

            if (OperatingSystem.IsWindows())
            {
                var programFiles =
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ProgramFiles);

                var programFilesX86 =
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ProgramFilesX86);

                var localAppData =
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData);

                candidates.AddRange(
                    new[]
                    {
                        Path.Combine(
                            programFiles,
                            "Google",
                            "Chrome",
                            "Application",
                            "chrome.exe"),

                        Path.Combine(
                            programFilesX86,
                            "Google",
                            "Chrome",
                            "Application",
                            "chrome.exe"),

                        Path.Combine(
                            localAppData,
                            "Google",
                            "Chrome",
                            "Application",
                            "chrome.exe"),

                        Path.Combine(
                            programFiles,
                            "Chromium",
                            "Application",
                            "chrome.exe"),

                        Path.Combine(
                            programFilesX86,
                            "Chromium",
                            "Application",
                            "chrome.exe")
                    });
            }
            else if (OperatingSystem.IsLinux())
            {
                candidates.AddRange(
                    new[]
                    {
                        "/usr/bin/google-chrome",
                        "/usr/bin/google-chrome-stable",
                        "/usr/bin/chromium",
                        "/usr/bin/chromium-browser",
                        "/snap/bin/chromium",
                        "/opt/google/chrome/chrome"
                    });
            }
            else if (OperatingSystem.IsMacOS())
            {
                candidates.Add(
                    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome");

                candidates.Add(
                    "/Applications/Chromium.app/Contents/MacOS/Chromium");
            }

            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            var pathEnvironment =
                Environment.GetEnvironmentVariable("PATH");

            if (!string.IsNullOrWhiteSpace(pathEnvironment))
            {
                var executableNames = OperatingSystem.IsWindows()
                    ? new[] { "chrome.exe", "chromium.exe" }
                    : new[]
                    {
                        "google-chrome",
                        "google-chrome-stable",
                        "chromium",
                        "chromium-browser"
                    };

                var pathExtensions = OperatingSystem.IsWindows()
                    ? (Environment.GetEnvironmentVariable("PATHEXT")
                       ?? ".EXE;.BAT;.CMD")
                        .Split(
                            ';',
                            StringSplitOptions.RemoveEmptyEntries)
                    : new[] { string.Empty };

                foreach (var directory in pathEnvironment.Split(
                             Path.PathSeparator,
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    foreach (var executableName in executableNames)
                    {
                        var executablePath =
                            Path.Combine(directory, executableName);

                        if (File.Exists(executablePath))
                        {
                            return executablePath;
                        }

                        if (OperatingSystem.IsWindows())
                        {
                            foreach (var extension in pathExtensions)
                            {
                                var pathWithExtension =
                                    Path.Combine(
                                        directory,
                                        Path.GetFileNameWithoutExtension(
                                            executableName) + extension);

                                if (File.Exists(pathWithExtension))
                                {
                                    return pathWithExtension;
                                }
                            }
                        }
                    }
                }
            }

            throw new FileNotFoundException(
                "Não foi possível localizar o Google Chrome ou Chromium. " +
                "Instale um desses navegadores ou defina a variável de " +
                "ambiente CHROME_BIN com o caminho completo do executável. " +
                $"Sistema operacional: {RuntimeInformation.OSDescription}.");
        }

        [Fact]
        public void AddPassengerPageTest()
        {
            _driver.Navigate()
                .GoToUrl(
                    "http://localhost:4200/enter-the-system");

            var emailField =
                _driver.FindElement(By.Id("email"));

            var passwordField =
                _driver.FindElement(By.Id("password"));

            var loginButton =
                _driver.FindElement(
                    By.CssSelector("button[type='submit']"));

            emailField.SendKeys(
                Environment.GetEnvironmentVariable(
                    "BUSTRACK_TEST_EMAIL")
                ?? throw new InvalidOperationException(
                    "A variável de ambiente BUSTRACK_TEST_EMAIL " +
                    "não foi definida."));

            passwordField.SendKeys(
                Environment.GetEnvironmentVariable(
                    "BUSTRACK_TEST_PASSWORD")
                ?? throw new InvalidOperationException(
                    "A variável de ambiente BUSTRACK_TEST_PASSWORD " +
                    "não foi definida."));

            loginButton.Click();

            AcceptAlertIfPresent();

            WaitForUrl("/dashboard");

            var passengersCard =
                WaitForElementClickable(
                    By.XPath(
                        "//button[contains(@class,'dashboard-card')]" +
                        "[.//h2[normalize-space()='Passageiros']]"),
                    TimeSpan.FromSeconds(10));

            Assert.NotNull(passengersCard);

            ScrollElementToCenter(passengersCard);

            ClickWithRetry(
                By.XPath(
                    "//button[contains(@class,'dashboard-card')]" +
                    "[.//h2[normalize-space()='Passageiros']]"),
                TimeSpan.FromSeconds(5));

            WaitForUrl("/passengers");

            Assert.EndsWith(
                "/passengers",
                _driver.Url);
        }

        private IWebElement WaitForElementClickable(
            By locator,
            TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow.Add(timeout);
            Exception? lastException = null;

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var element = _driver.FindElement(locator);

                    if (element.Displayed && element.Enabled)
                    {
                        return element;
                    }
                }
                catch (NoSuchElementException exception)
                {
                    lastException = exception;
                }
                catch (StaleElementReferenceException exception)
                {
                    lastException = exception;
                }

                Thread.Sleep(200);
            }

            throw new WebDriverTimeoutException(
                $"O elemento '{locator}' não ficou disponível para clique.",
                lastException);
        }

        private void ScrollElementToCenter(IWebElement element)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript(
                "arguments[0].scrollIntoView({" +
                "behavior: 'instant'," +
                "block: 'center'," +
                "inline: 'center'" +
                "});",
                element);

            Thread.Sleep(200);
        }

        private void ClickWithRetry(
            By locator,
            TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow.Add(timeout);
            Exception? lastException = null;

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var element = WaitForElementClickable(
                        locator,
                        TimeSpan.FromSeconds(2));

                    ScrollElementToCenter(element);
                    element.Click();

                    return;
                }
                catch (ElementClickInterceptedException exception)
                {
                    lastException = exception;
                }
                catch (StaleElementReferenceException exception)
                {
                    lastException = exception;
                }

                Thread.Sleep(300);
            }

            throw new WebDriverTimeoutException(
                $"Não foi possível clicar no elemento '{locator}' " +
                "dentro do tempo limite.",
                lastException);
        }

        private void AcceptAlertIfPresent()
        {
            var timeout = DateTime.UtcNow.AddSeconds(5);

            while (DateTime.UtcNow < timeout)
            {
                try
                {
                    var alert = _driver.SwitchTo().Alert();

                    alert.Accept();

                    return;
                }
                catch (NoAlertPresentException)
                {
                    Thread.Sleep(250);
                }
            }
        }

        private void WaitForUrl(string expectedPath)
        {
            var timeout = DateTime.UtcNow.AddSeconds(10);

            while (DateTime.UtcNow < timeout)
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
            try
            {
                _driver.Quit();
            }
            finally
            {
                _driver.Dispose();
            }
        }
    }
}
