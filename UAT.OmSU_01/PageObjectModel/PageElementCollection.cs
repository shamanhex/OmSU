using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using ITSK.UAT.Aliasing;

namespace ITSK.Selenium.PageObjectModel
{
    public class PageElementCollection : PageElementCore, IEnumerable<PageElement>
    {
        public virtual ICollection<PageElement> Elements
        {
            get
            {
                if (this._elements == null)
                {
                    try
                    {
                        var webElements = this.Page.FindWebElements(this.XPath, this.WaitTimeout);

                        List<PageElement> pageElements = new List<PageElement>();

                        foreach (IWebElement webElement in webElements)
                        {
                            var item = new PageElement(this.Page, null);
                            item.Element = webElement;

                            pageElements.Add(item);
                        }

                        this._elements = pageElements.AsReadOnly();
                    }
                    catch (NoSuchElementException ex)
                    {
                        throw new InvalidOperationException(string.Format("Элемент не найден (xPath: {0}). Ошибка: {1}", XPath, ex.Message), ex);
                    }
                    catch (ElementNotVisibleException ex)
                    {
                        throw new InvalidOperationException(string.Format("Элемент не отображается (xPath: {0}). Ошибка: {1}", XPath, ex.Message), ex);
                    }
                }
                return this._elements;
            }
        }
        
        private ICollection<PageElement> _elements = null;

        public PageElementCollection(PageCore page, string xPath) : base(page, xPath) { }


        public IEnumerator<PageElement> GetEnumerator()
        {
            return this.Elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this.Elements).GetEnumerator();
        }

        public override bool IsDisplayed
        {
            get
            {
                return this.Where(x => x.IsDisplayed).Count() > 0;
            }
        }

        public static PageElementCollection GetElementCollectionByAlias(string groupAlias)
        {
            PageCore currentPage = ScenarioContext.Current.Page();
            PageElement currentElement = ScenarioContext.Current.Element();
            PageElementCollection element = GetElementCollectionByAlias(currentPage, currentElement, groupAlias);
            return element;
        }

        public static PageElementCollection GetElementCollectionByAlias(PageCore page, PageElement currentElement, string groupAlias)
        {
            if (page == null)
                throw new ArgumentException("Страница на которой происходит поиск не может иметь пустое значение");

            PageElementCore element = null;

            if (currentElement != null && currentElement.TryFindElementByAlias(groupAlias, out element))
            {
                PageElementCollection collection = element as PageElementCollection;
                if (collection != null)
                    return collection;
            }
            if (page.TryFindElementByAlias(groupAlias, out element))
            {
                PageElementCollection collection = element as PageElementCollection;
                if (collection != null)
                    return collection;
            }

            string xPath = Alias.ResolveAsString(groupAlias);

            return page.FindElementCollection(xPath);
        }
    }
}
