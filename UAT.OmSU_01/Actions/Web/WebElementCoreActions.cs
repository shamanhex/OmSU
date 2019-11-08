using TechTalk.SpecFlow;
using ITSK.AssemblyAllureAdapter.Attributes;
using NUnit.Framework;
using ITSK.Selenium.PageObjectModel;

namespace UAT.Terra.Actions.Web
{
    [Binding]
    public static class WebElementCoreActions
    {
        [StepDefinition(@"элемент ""(.*)"" присутствует на странице")]
        [StepDefinition(@"""(.*)"" присутствует на странице")]
        public static void ElementExistInPage(string alias)
        {
            PageElementCore element = PageElementCore.GetElementByAlias(alias);
            ScenarioContext.Current.SetCurrent(element);
            Assert.IsTrue(element.IsDisplayed, string.Format("Элемент '{0}' (xPath: {1}) не найден на странице.", alias, element.XPath));
        }

        [StepDefinition(@"элемент ""(.*)"" отсутствует на странице")]
        [StepDefinition(@"""(.*)"" отсутствует на странице")]
        [StepDefinition(@"элемент ""(.*)"" не отображается на странице")]
        [StepDefinition(@"""(.*)""  не отображается на странице")]
        public static void ElementNotExistInPage(string alias)
        {
            PageElementCore element = PageElementCore.GetElementByAlias(alias);
            ScenarioContext.Current.SetCurrent(element);
            Assert.IsFalse(element.IsDisplayed, string.Format("Элемент '{0}' (xPath: {1}) не найден на странице.", alias, element.XPath));
        }

        [StepDefinition(@"""(.*)"" отображается на странице: (.*)")]
        public static void IsElementVisible(string alias, bool isVisible)
        {
            PageElementCore element = PageElementCore.GetElementByAlias(alias);
            ScenarioContext.Current.SetCurrent(element);
            Assert.AreEqual(element.IsDisplayed, isVisible, string.Format("Ожидалось, что видимость элемента '{0}' (xPath: {1}) будет: {2} ", alias, element.XPath, isVisible));
        }
    }
}
