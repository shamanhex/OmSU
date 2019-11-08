using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace UAT.Terra.Actions.Web
{
    [Binding]
    public class DriversStub
    {
        [StepDefinition(@"пользователь использует браузер Chrome")]
        public void UserUseChrome()
        {
            IWebDriver driver = new ChromeDriver();
            ScenarioContext.Current["Driver"] = driver;
        }

        [StepDefinition(@"пользователь использует браузер Internet Explorer")]
        public void UserUseIE()
        {
            IWebDriver driver = new InternetExplorerDriver();
            ScenarioContext.Current["Driver"] = driver;
        }

        [StepDefinition(@"Selenium Grid: (.*)")]
        public void UserUseGrid(string address)
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new RemoteWebDriver(new Uri(address), options.ToCapabilities(), TimeSpan.FromMinutes(30));
            ScenarioContext.Current["Driver"] = driver;
        }

        [AfterScenario]
        public void CloseBrowser()
        {
            IWebDriver driver = ScenarioContext.Current["Driver"] as IWebDriver;

            if (driver != null)
            {
                Console.WriteLine("Закрыть браузер");
                driver.Close();
            }
        }
    }
}
