using ITSK.Selenium.PageObjectModel;
using ITSK.UAT.Aliasing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace UAT.Terra.Actions.Web
{
    [Binding]
    public static class WebElementActions
    {

        [StepDefinition(@"на странице отображаются элементы:")]
        public static void ElementsExistInPage(Table elements)
        {
            foreach (TableRow elementRow in elements.Rows)
            {
                string elementAlias = elementRow[0];
                PageElement element = PageElement.GetElementByAlias(elementAlias);

                Assert.IsTrue(element.IsDisplayed, string.Format("Элемент (xPath: {0}) не найден на странице.", element.XPath));
            }
        }

        [StepDefinition(@"текущий элемент ""(.*)""")]
        [StepDefinition(@"рассмотрим элемент ""(.*)""")]
        public static void SelectCurrentElement(string alias)
        {
            PageElement element = PageElement.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);
        }


        [StepDefinition(@"кликнул по элементу ""(.*)""")]
        [StepDefinition(@"пользователь кликнул по элементу ""(.*)""")]
        [StepDefinition(@"нажал на элемент ""(.*)""")]
        [StepDefinition(@"пользователь нажал на элемент ""(.*)""")]
        [StepDefinition(@"нажал на кнопку ""(.*)""")]
        [StepDefinition(@"нажал на ссылку ""(.*)""")]
        public static void ClickByElement(string alias)
        {
            PageElement element = PageElementCore.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);

            element.Click();
        }

        [StepDefinition(@"кликнул по элементу")]
        [StepDefinition(@"нажал на элемент")]
        [StepDefinition(@"нажал на кнопку")]
        [StepDefinition(@"нажал на ссылку")]
        public static void ClickByCurrenctElement()
        {
            PageElement element = ScenarioContext.Current.Element();

            element.Click();
        }

        [StepDefinition(@"навёл курсор на ""(.*)""")]
        [StepDefinition(@"пользователь навёл курсор на ""(.*)""")]
        [StepDefinition(@"навёл мышку на ""(.*)""")]
        [StepDefinition(@"пользователь навёл мышку на ""(.*)""")]
        public static void MoveCursorToElement(string alias)
        {
            PageElement element = PageElementCore.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);

            element.MoveMouseTo(0, 0);
        }

        [StepDefinition(@"очистил поле ""(.*)""")]
        [StepDefinition(@"очистил ""(.*)""")]
        public static void ClearElement(string alias)
        {
            PageElement element = PageElement.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);

            element.Clear();
        }

        [StepDefinition(@"очистил поле")]
        [StepDefinition(@"очистил")]
        public static void ClearCurrenctElement()
        {
            PageElement element = ScenarioContext.Current.Element();

            element.Clear();
        }

        [StepDefinition(@"пользователь ввёл текст ""(.*)"" в поле ""(.*)""")]
        [StepDefinition(@"ввёл текст ""(.*)"" в поле ""(.*)""")]
        public static void EnterTextToInput(string textAlias, string alias)
        {
            PageElement element = PageElement.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);
            string text = Alias.ResolveAsString(textAlias);

            element.Clear();
            element.TypeText(text);
        }

        [StepDefinition(@"ввёл текст ""(.*)""")]
        public static void EnterTextToCurrentElement(string textAlias)
        {
            PageElement currentElement = ScenarioContext.Current.Element();
            string text = Alias.ResolveAsString(textAlias);

            currentElement.Clear();
            currentElement.TypeText(text);
        }

        [StepDefinition(@"нажал Enter")]
        [StepDefinition(@"нажал Ввод")]
        public static void PressEnter()
        {
            PageElement currentElement = ScenarioContext.Current.Element();

            currentElement.PressEnter();
        }

        [StepDefinition(@"нажал Tab")]
        public static void PressTab()
        {
            PageElement currentElement = ScenarioContext.Current.Element();

            currentElement.PressTab();
        }


        [StepDefinition(@"элемент ""(.*)"" содержит текст ""(.*)""")]
        [StepDefinition(@"поле ""(.*)"" содержит текст ""(.*)""")]
        [StepDefinition(@"""(.*)"" содержит текст ""(.*)""")]
        public static void ElementContainsText(string alias, string textAlias)
        {
            PageElement element = PageElement.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);
            string text = Alias.ResolveAsString(textAlias);

            text = text.ToLower();

            StringAssert.Contains(element.TextAsLine.ToLower(), text, string.Format("Элемент '{0}' не содержит текст '{1}'. Текст элемента: '{2}'", alias, text, element.TextAsLine));
        }

        [StepDefinition(@"элемент ""(.*)"" содержит текст ""(.*)"" с учётом регистра")]
        [StepDefinition(@"поле ""(.*)"" содержит текст ""(.*)"" с учётом регистра")]
        [StepDefinition(@"""(.*)"" содержит текст ""(.*)"" с учётом регистра")]
        public static void ElementContainsTextСaseSensitive(string alias, string textAlias)
        {
            PageElement element = PageElement.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);
            string text = Alias.ResolveAsString(textAlias);

            StringAssert.Contains(element.TextAsLine, text, string.Format("Элемент '{0}' не содержит текст '{1}'. Текст элемента: '{2}'", alias, text, element.TextAsLine));
        }

        [StepDefinition(@"содержит текст ""(.*)""")]
        public static void CurrentElementContainsText(string textAlias)
        {
            PageElement element = ScenarioContext.Current.Element();
            string text = Alias.ResolveAsString(textAlias);

            text = text.ToLower();

            StringAssert.Contains(element.TextAsLine.ToLower(), text, string.Format("Текущий элемент не содержит текст '{0}'. Текст элемента: '{1}'", text, element.TextAsLine));
        }

        [StepDefinition(@"содержит текст ""(.*)"" c учётом регистра")]
        public static void CurrentElementContainsTextСaseSensitive(string textAlias)
        {
            PageElement element = ScenarioContext.Current.Element();
            string text = Alias.ResolveAsString(textAlias);

            StringAssert.Contains(element.TextAsLine, text, string.Format("Текущий элемент не содержит текст '{0}'. Текст элемента: '{1}'", text, element.TextAsLine));
        }

        [StepDefinition(@"текст элемента ""(.*)"" соответствует регулярному выражению ""(.*)""")]
        public static void ElementTextMatchRegex(string alias, string patternAlias)
        {
            PageElement element = PageElement.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);
            string pattern = Alias.ResolveAsString(patternAlias);

            Regex regex = new Regex(pattern);

            Assert.IsTrue(regex.IsMatch(element.TextAsLine), string.Format("Текст в элементе '{0}' не совпадает с шаблоном '{1}'. Текст элемента: '{2}'", alias, pattern, element.TextAsLine));
        }

        [StepDefinition(@"регулярное выражение ""(.*)"" соответствует тексту элемента")]
        public static void CurrentElementTextMatchRegex(string patternAlias)
        {
            PageElement element = ScenarioContext.Current.Element();
            string pattern = Alias.ResolveAsString(patternAlias);

            Regex regex = new Regex(pattern);

            Assert.IsTrue(regex.IsMatch(element.TextAsLine), string.Format("Текст в текущем элементе не совпадает с шаблоном '{0}'. Текст элемента: '{1}'", pattern, element.TextAsLine));
        }

        [StepDefinition(@"элемент ""(.*)"" является ссылкой")]
        [StepDefinition(@"""(.*)"" является ссылкой")]
        public static void ElementIsLink(string alias)
        {
            PageElement element = PageElement.GetElementByAlias(alias);
            ScenarioContext.Current.SetElement(element);
        }
    }
}
