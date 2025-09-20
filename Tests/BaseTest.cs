using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace SeleniumCSharpSample.Tests
{
    public class BaseTest
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;
        protected string sessionId;

        /// <summary>
        /// Creates a LambdaTest remote driver using LT:Options, with debug artifacts enabled.
        /// </summary>
        protected IWebDriver CreateRemoteDriver(string browserName, string browserVersion, string platformName, string testName)
        {
            var user = Environment.GetEnvironmentVariable("LT_USERNAME");
            var key = Environment.GetEnvironmentVariable("LT_ACCESS_KEY");

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("LT_USERNAME or LT_ACCESS_KEY is not set in environment variables.");
            }

            ChromeOptions options = new ChromeOptions();
            options.BrowserVersion = browserVersion;

            // LambdaTest W3C style options
            var ltOptions = new Dictionary<string, object>()
            {
                {"username", user},
                {"accessKey", key},
                {"platformName", platformName},
                {"project", "SeleniumCSharpCert"},
                {"build", "CertBuild-1"},
                {"name", testName},
                {"w3c", true},
                // required debugging artifacts:
                {"network", true},
                {"console", true},
                {"video", true},
                {"visual", true}
            };

            options.AddAdditionalOption("LT:Options", ltOptions);

            var remoteUrl = new Uri("https://hub.lambdatest.com/wd/hub/");

            // set generous command timeout to allow network/video capture
            var remoteDriver = new RemoteWebDriver(remoteUrl, options.ToCapabilities(), TimeSpan.FromSeconds(600));
            driver = remoteDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20)); // test timeout 20s as required

            sessionId = ((RemoteWebDriver)driver).SessionId.ToString();
            Console.WriteLine($"[LT] Session ID: {sessionId}");

            // append session id to file for submission
            try
            {
                File.AppendAllText("session_ids.txt", $"{sessionId}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not write session_ids.txt: {ex.Message}");
            }

            return driver;
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                var passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
                // mark status on LambdaTest dashboard (optional)
                try
                {
                    ((IJavaScriptExecutor)driver)?.ExecuteScript($"lambda-status={(passed ? "passed" : "failed")}");
                }
                catch { /* ignore */ }
            }
            catch { /* ignore */ }
            finally
            {
                try { driver?.Quit(); } catch { }
            }
        }
    }
}
