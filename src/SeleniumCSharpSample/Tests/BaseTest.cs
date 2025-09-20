using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;

namespace SeleniumCSharpSample.Tests
{
    public class BaseTest
    {
        protected IWebDriver? driver;
        protected WebDriverWait? wait;
        protected string? sessionId;

        // Browser + OS combinations for data-driven tests
        public static IEnumerable<TestCaseData> BrowserConfigs()
        {
            yield return new TestCaseData("chrome", "128.0", "Windows 10");
            yield return new TestCaseData("edge", "127.0", "macOS Ventura");
            yield return new TestCaseData("firefox", "130.0", "Windows 11");
            yield return new TestCaseData("internet explorer", "11.0", "Windows 10");
        }

        protected IWebDriver CreateRemoteDriver(string browser, string version, string platform, string testName)
        {
            // Ensure environment variables are set
            string username = Environment.GetEnvironmentVariable("LT_USERNAME");
            string accessKey = Environment.GetEnvironmentVariable("LT_ACCESS_KEY");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(accessKey))
                throw new InvalidOperationException("LT_USERNAME or LT_ACCESS_KEY is not set in environment variables.");

            ICapabilities capabilities = null;

            switch (browser.ToLower())
            {
                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.BrowserVersion = version;
                    chromeOptions.PlatformName = platform;
                    chromeOptions.AddAdditionalOption("LT:Options", new Dictionary<string, object>
                    {
                        ["build"] = "LambdaTest C# 101",
                        ["name"] = testName,
                        ["network"] = true,
                        ["video"] = true,
                        ["console"] = true,
                        ["tunnel"] = false
                    });
                    capabilities = chromeOptions.ToCapabilities();
                    break;

                case "edge":
                    var edgeOptions = new EdgeOptions();
                    edgeOptions.BrowserVersion = version;
                    edgeOptions.PlatformName = platform;
                    edgeOptions.AddAdditionalOption("LT:Options", new Dictionary<string, object>
                    {
                        ["build"] = "LambdaTest C# 101",
                        ["name"] = testName,
                        ["network"] = true,
                        ["video"] = true,
                        ["console"] = true,
                        ["tunnel"] = false
                    });
                    capabilities = edgeOptions.ToCapabilities();
                    break;

                case "firefox":
                    var firefoxOptions = new FirefoxOptions();
                    firefoxOptions.BrowserVersion = version;
                    firefoxOptions.PlatformName = platform;
                    firefoxOptions.AddAdditionalOption("LT:Options", new Dictionary<string, object>
                    {
                        ["build"] = "LambdaTest C# 101",
                        ["name"] = testName,
                        ["network"] = true,
                        ["video"] = true,
                        ["console"] = true,
                        ["tunnel"] = false
                    });
                    capabilities = firefoxOptions.ToCapabilities();
                    break;

                case "internet explorer":
                    var ieOptions = new InternetExplorerOptions();
                    ieOptions.BrowserVersion = version;
                    ieOptions.PlatformName = platform;
                    ieOptions.AddAdditionalOption("LT:Options", new Dictionary<string, object>
                    {
                        ["build"] = "LambdaTest C# 101",
                        ["name"] = testName,
                        ["network"] = true,
                        ["video"] = true,
                        ["console"] = true,
                        ["tunnel"] = false
                    });
                    capabilities = ieOptions.ToCapabilities();
                    break;

                default:
                    throw new ArgumentException("Browser not supported: " + browser);
            }

            // Remote WebDriver URL
            string hubUrl = $"https://{username}:{accessKey}@hub.lambdatest.com/wd/hub";
            driver = new RemoteWebDriver(new Uri(hubUrl), capabilities, TimeSpan.FromSeconds(600));

            // Set wait
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Get session ID for LambdaTest
            sessionId = ((RemoteWebDriver)driver).SessionId.ToString();
            Console.WriteLine("LambdaTest Session ID: " + sessionId);

            return driver;
        }
    }
}
