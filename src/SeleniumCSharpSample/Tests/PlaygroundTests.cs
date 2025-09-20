using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpSample.Tests
{
    [TestFixture]
    public class PlaygroundTests : BaseTest
    {
        [Test, TestCaseSource(nameof(BrowserConfigs))]
        public void Scenario1_SimpleFormDemo(string browser, string version, string platform)
        {
            string testName = TestContext.CurrentContext.Test.Name;
            driver = CreateRemoteDriver(browser, version, platform, testName);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // 1. Open Playground
            driver.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");

            // 2. Click "Simple Form Demo"
            wait.Until(d => d.FindElement(By.LinkText("Simple Form Demo"))).Click();

            // 3. Validate URL
            Assert.IsTrue(driver.Url.Contains("simple-form-demo"));

            // 4. Enter message
            string message = "Welcome to LambdaTest";
            var inputBox = wait.Until(d => d.FindElement(By.Id("user-message")));
            inputBox.Clear();
            inputBox.SendKeys(message);

            // 5. Click "Get Checked Value"
            var getValueBtn = wait.Until(d =>
            {
                IWebElement btn = null;
                // Try multiple locators
                try { btn = d.FindElement(By.Id("showInput")); } catch { }
                try { if (btn == null) btn = d.FindElement(By.CssSelector(".btn.btn-default")); } catch { }
                try { if (btn == null) btn = d.FindElement(By.XPath("//button[text()='Get Checked Value']")); } catch { }
                return btn;
            });
            getValueBtn.Click();

            // 6. Validate output
            var output = wait.Until(d => d.FindElement(By.Id("message")));
            Assert.AreEqual(message, output.Text);

            driver.Quit();
        }

        [Test, TestCaseSource(nameof(BrowserConfigs))]
        public void Scenario2_DragDropSliders(string browser, string version, string platform)
        {
            string testName = TestContext.CurrentContext.Test.Name;
            driver = CreateRemoteDriver(browser, version, platform, testName);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            driver.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            wait.Until(d => d.FindElement(By.LinkText("Drag & Drop Sliders"))).Click();

            var slider = wait.Until(d => d.FindElement(By.XPath("//input[@value='15']")));
            int targetValue = 95;

            // Move slider using Actions
            var actions = new OpenQA.Selenium.Interactions.Actions(driver);
            while (int.Parse(slider.GetAttribute("value")) < targetValue)
            {
                actions.ClickAndHold(slider).MoveByOffset(5, 0).Release().Perform();
            }

            // Validate final value
            Assert.AreEqual(targetValue.ToString(), slider.GetAttribute("value"));

            driver.Quit();
        }

        [Test, TestCaseSource(nameof(BrowserConfigs))]
        public void Scenario3_InputFormSubmit(string browser, string version, string platform)
        {
            string testName = TestContext.CurrentContext.Test.Name;
            driver = CreateRemoteDriver(browser, version, platform, testName);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            driver.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            wait.Until(d => d.FindElement(By.LinkText("Input Form Submit"))).Click();

            // 2. Click Submit without filling
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // 3. Validate error message
            var nameField = driver.FindElement(By.Name("name"));
            Assert.IsTrue(nameField.GetAttribute("validationMessage").Contains("Please fill"));

            // 4. Fill required fields
            driver.FindElement(By.Name("name")).SendKeys("Test User");
            driver.FindElement(By.Name("email")).SendKeys("test@example.com");
            driver.FindElement(By.Name("password")).SendKeys("Password123!");
            driver.FindElement(By.Name("company")).SendKeys("LambdaTest");
            driver.FindElement(By.Name("website")).SendKeys("https://lambdatest.com");
            var countryDropdown = new SelectElement(driver.FindElement(By.Name("country")));
            countryDropdown.SelectByText("United States");
            driver.FindElement(By.Name("city")).SendKeys("San Francisco");
            driver.FindElement(By.Name("address1")).SendKeys("123 Main St");
            driver.FindElement(By.Name("state")).SendKeys("CA");
            driver.FindElement(By.Name("zipcode")).SendKeys("94105");

            // 6. Submit form
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // 7. Validate success message
            var successMsg = wait.Until(d => d.FindElement(By.ClassName("success-msg")));
            Assert.IsTrue(successMsg.Text.Contains("Thanks for contacting us"));

            driver.Quit();
        }
    }
}
