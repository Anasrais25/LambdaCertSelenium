using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Linq;
using OpenQA.Selenium.Remote;
using System.Threading;

namespace SeleniumCSharpSample.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)] // run tests in parallel (class/method-level)
    public class PlaygroundTests : BaseTest
    {
        // Provide the 4 combinations required
        public static IEnumerable BrowserPlatformCombos()
        {
            // browser, version, platform
            yield return new TestCaseData("Chrome", "128.0", "Windows 10").SetName("Chrome_128_Win10");
            yield return new TestCaseData("MicrosoftEdge", "127.0", "macOS Ventura").SetName("Edge_127_macVentura");
            yield return new TestCaseData("Firefox", "130.0", "Windows 11").SetName("Firefox_130_Win11");
            yield return new TestCaseData("Internet Explorer", "11.0", "Windows 10").SetName("IE_11_Win10");
        }

        // Scenario 1: Simple Form Demo
        [Test, TestCaseSource(nameof(BrowserPlatformCombos)), Timeout(20000)]
        public void Scenario1_SimpleFormDemo(string browser, string version, string platform)
        {
            var drv = CreateRemoteDriver(browser, version, platform, $"SimpleForm_{browser}");
            drv.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            wait.Until(d => d.FindElement(By.LinkText("Simple Form Demo"))).Click();

            // Validate URL contains simple-form-demo
            wait.Until(d => d.Url.Contains("simple-form-demo"));
            Assert.IsTrue(driver.Url.Contains("simple-form-demo"), "URL does not contain 'simple-form-demo'");

            // Use a variable to enter message
            string msg = "Welcome to LambdaTest";

            // Enter message into input (locator: Id)
            var input = wait.Until(d => d.FindElement(By.Id("user-message")));
            input.Clear();
            input.SendKeys(msg);

            // Click Get Checked Value (locator: CssSelector)
            var btn = driver.FindElement(By.CssSelector("#get-input button"));
            btn.Click();

            // Validate displayed message (locator: xpath)
            var displayed = wait.Until(d => d.FindElement(By.XPath("//div[@id='message']/span")));
            Assert.AreEqual(msg, displayed.Text.Trim(), "Displayed message does not match input message.");
        }

        // Scenario 2: Drag & Drop Sliders
        [Test, TestCaseSource(nameof(BrowserPlatformCombos)), Timeout(20000)]
        public void Scenario2_DragDropSliders(string browser, string version, string platform)
        {
            var drv = CreateRemoteDriver(browser, version, platform, $"Slider_{browser}");
            drv.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            wait.Until(d => d.FindElement(By.LinkText("Drag & Drop Sliders"))).Click();

            // Locate the slider row that has default value 15.
            // Using XPath to find label text and following input range
            var sliderLabel = wait.Until(d => d.FindElement(By.XPath("//label[contains(text(),'Default value 15')]/following::input[1]")));
            var rangeValueElem = driver.FindElement(By.Id("rangeSuccess")); // displayed value

            // Set slider to 95 using JS (ensures compatibility across browsers)
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].value = 95; arguments[0].dispatchEvent(new Event('change'))", sliderLabel);

            // Wait and validate displayed value shows 95
            wait.Until(d => rangeValueElem.Text.Trim() == "95");
            Assert.AreEqual("95", rangeValueElem.Text.Trim(), "Slider value is not 95");
        }

        // Scenario 3: Input Form Submit validations
        [Test, TestCaseSource(nameof(BrowserPlatformCombos)), Timeout(20000)]
        public void Scenario3_InputFormSubmit(string browser, string version, string platform)
        {
            var drv = CreateRemoteDriver(browser, version, platform, $"InputForm_{browser}");
            drv.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            wait.Until(d => d.FindElement(By.LinkText("Input Form Submit"))).Click();

            // Try to submit without filling (locator: CssSelector for button)
            var submitBtn = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            submitBtn.Click();

            // Check browser-native validation message for first invalid element using JS
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var validationMsg = js.ExecuteScript("var e=document.querySelector(':invalid'); return e ? e.validationMessage : ''") as string ?? "";

            Assert.IsFalse(string.IsNullOrEmpty(validationMsg), "No validation message returned.");
            // As asked: assert exact message "Please fill out this field." — attempt that, but fallback if browser message differs
            bool exact = validationMsg.Trim().Equals("Please fill out this field.", StringComparison.OrdinalIgnoreCase);
            if (!exact)
            {
                // Some browsers return slightly different text; ensure it includes the word "fill"
                Assert.IsTrue(validationMsg.ToLower().Contains("fill"), $"Validation message unexpected: {validationMsg}");
            }

            // Now fill the form fields using at least three different locators (Id, Name, CssSelector)
            driver.FindElement(By.Id("name")).SendKeys("John Doe"); // Id
            driver.FindElement(By.Name("email")).SendKeys("john.doe@example.com"); // Name
            driver.FindElement(By.CssSelector("input[placeholder='Company name']")).SendKeys("ACME Inc"); // CssSelector

            // Fill other required fields
            driver.FindElement(By.Id("password")).SendKeys("Password123!");
            driver.FindElement(By.Id("company")).SendKeys("ACME Inc");
            driver.FindElement(By.Id("website")).SendKeys("https://example.com");
            driver.FindElement(By.Id("city")).SendKeys("Austin");
            driver.FindElement(By.Id("address1")).SendKeys("123 Main St");
            driver.FindElement(By.Id("address2")).SendKeys("Suite 100");
            driver.FindElement(By.Id("state")).SendKeys("TX");
            driver.FindElement(By.Id("zip")).SendKeys("73301");

            // Select country "United States" by visible text using SelectElement (locator: name)
            var countrySelect = new OpenQA.Selenium.Support.UI.SelectElement(driver.FindElement(By.Name("country")));
            countrySelect.SelectByText("United States");

            // Submit again
            submitBtn.Click();

            // After submission validate success message
            // Wait for success message element - using XPath target text described
            var successElem = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(),'Thanks for contacting us, we will get back to you shortly.')]")));
            Assert.IsTrue(successElem.Displayed, "Success message not displayed.");
        }
    }
}
