using ITSK.Selenium.PageObjectModel;
using ITSK.UAT.Aliasing;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace UAT.Terra.Actions.Web
{
    [Binding]
    public class PageManipulationActions
    {
        [StepDefinition(@"пользователь открыл страницу ""(.*)""")]
        [StepDefinition(@"открыл страницу ""(.*)""")]
        public void OpenPage(string pageAlias)
        {
            if ((IWebDriver)ScenarioContext.Current["Driver"] == null)
            {
                throw new InvalidOperationException("ScenarioContext.Current[\"Driver\"] is null");
            }

            PageCore page = null;
            page = PageCore.TryResolveAsPage(pageAlias);
            if (page == null)// && a != null)
            {
                page = new PageCore();
                string url = Alias.ResolveAsString(pageAlias);
                page.StartUrl = url;
            }

            ScenarioContext.Current.SetPage(page);
            
            page.Open();
        }

        [StepDefinition(@"изменил размеры окна до (.*) на (.*) px")]
        public void SetPageSize(int width, int height)
        {
            PageCore currentPage = ScenarioContext.Current.Page();
            currentPage.SetWindowSize(width, height);
        }

        [StepDefinition(@"выделил элемент ""(.*)""")]
        public void SelectRegion(string alias)
        {
            this.SelectRegion(alias, alias);
        }

        [StepDefinition(@"выделил область от ""(.*)"" до ""(.*)""")]
        public void SelectRegion(string fromAlias, string toAlias)
        {
            PageCore currentPage = ScenarioContext.Current.Page();

            string fromXPath = Alias.ResolveAsString(fromAlias);
            PageElement from = currentPage.FindElement(fromXPath);

            string toXPath = Alias.ResolveAsString(toAlias);
            PageElement to = currentPage.FindElement(toXPath);

            from.MoveMouseTo(1, 1);
            from.MouseDown(1, 1);

            //to.MoveMouseTo(2, 2);
            to.MouseUp(1, 1);
        }

        [StepDefinition(@"переместил окно в точку (.*), (.*)")]
        public void MovePageWindowTo(int x, int y)
        {
            PageCore currentPage = ScenarioContext.Current.Page();
            currentPage.MoveWindowTo(x, y);
        }

        [StepDefinition(@"развернул окно страницы на весь экран")]
        public void MaximizePageWindow()
        {
            PageCore currentPage = ScenarioContext.Current.Page();
            currentPage.MaximizeWindow();
        }

        [StepDefinition(@"обновил страницу")]
        [StepDefinition(@"пользователь обновил страницу")]
        public void RefreshWindow()
        {
            PageCore currentPage = ScenarioContext.Current.Page();
            currentPage.Refresh();
        }

        [StepDefinition(@"пользователь перешел на страницу ""(.*)""")]
        public void GoToNewPage(string pageAlias)
        {

            string targetPage = Alias.ResolveAsString(pageAlias);
            PageCore currentPage = ScenarioContext.Current.Page();

            Assert.IsTrue(currentPage.Url.Equals(targetPage), string.Format("Пользователь не перешел на страницу '{0}'.", targetPage));
        }
    }
}
