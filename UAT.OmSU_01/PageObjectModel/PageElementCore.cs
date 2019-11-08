using ITSK.UAT.Aliasing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace ITSK.Selenium.PageObjectModel
{
    public abstract class PageElementCore
    {
        public virtual string XPath { get; set; }
        public virtual PageCore Page { get; set; }
        public virtual TimeSpan WaitTimeout
        {
            get
            {
                if (_waitTimeout == TimeSpan.Zero)
                {
                    _waitTimeout = this.Page.GetPageTimeoutSpan();
                    if (_waitTimeout == TimeSpan.Zero)
                        _waitTimeout = TimeSpan.FromMilliseconds(100);
                }
                return _waitTimeout;
            }
            set
            {
                _waitTimeout = value;
            }
        }
        private TimeSpan _waitTimeout = TimeSpan.Zero;

        public PageElementCore(PageCore page, string xPath)
        {
            this.Page = page;
            this.XPath = xPath;
        }

        public abstract bool IsDisplayed { get; }

        public static PageElement GetElementByAlias(PageCore page, PageElement currentElement, string alias)
        {
            if (page == null)
                throw new ArgumentException("Страница на которой происходит поиск не может иметь пустое значение");

            PageElementCore elementCore = null;

            if (currentElement != null && currentElement.TryFindElementByAlias(alias, out elementCore))
            {
                PageElement element = elementCore as PageElement;
                if (element != null)
                    return element;
            }

            if (page.TryFindElementByAlias(alias, out elementCore))
            {
                PageElement element = elementCore as PageElement;
                if (element != null)
                    return element;
            }

            string xPath = Alias.ResolveAsString(alias);

            return page.FindElement(xPath);
        }

        public static PageElement GetElementByAlias(string alias)
        {
            PageCore currentPage = ScenarioContext.Current.Page();
            PageElement currentElement = ScenarioContext.Current.Element();
            PageElement element = PageElement.GetElementByAlias(currentPage, currentElement, alias);
            return element;
        }

        public virtual T TryExecute<T>(Func<IWebDriver, T> action)
        {
            WebDriverWait wait = new WebDriverWait(this.Page.Driver, this.WaitTimeout);
            return wait.Until(driver =>
            {
                try
                {
                    return action.Invoke(driver);
                }
                catch (StaleElementReferenceException ex)
                {
                    throw new NoSuchElementException(ex.Message, ex);
                }
            });
        }

        public override string ToString()
        {
            return XPath;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
