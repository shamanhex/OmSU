using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ITSK.AssemblyAllureAdapter.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;
using ITSK.UAT.Aliasing;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System.Web;

namespace ITSK.Selenium.PageObjectModel
{
    public class PageCore
    {
        public virtual string StartUrl { get; set; }

        private IWebDriver _driver = null;

        public virtual IWebDriver Driver
        {
            get
            {
                if (this._driver == null)
                {
                    //this._driver = Browser.Current;
                    ////***************************************
                    this._driver = (IWebDriver)ScenarioContext.Current["Driver"];
                    ////*******************************************
                    TimeSpan timeout = ScenarioContext.Current.WaitTimeout();
                    if (timeout > TimeSpan.Zero)
                    {
                        this._driver.Manage().Timeouts().ImplicitWait = timeout;
                    }
                }
                return this._driver;
            }
            set
            {
                this._driver = value;
            }
        }

        public PageCore() 
        {
            Alias.RefreshAliases(typeof(PageCore));
            InitElements();
        }

        public PageCore(IWebDriver driver)
        {
            this._driver = driver;
            InitElements();
        }
        
        public virtual PageCore Open()
        {
            try
            {
                return this.GoTo(this.StartUrl);
            }
            catch (WebDriverException ex)
            {
                throw new InvalidOperationException("Время ожидания ответа на HTTP запрос к удалённому WebDriver истекло (timeout: 60 sec)", ex);
            }
        }

        public virtual PageCore GoTo(string url)
        {
            //make full url
            url = Alias.ResolveFullUrl(url);
            //add credentials
            var handle = this.Driver.CurrentWindowHandle;
            bool isIE = checkIsIE(this.Driver);

            if (!string.IsNullOrEmpty(ScenarioContext.Current.User()))
            {
                string user = ScenarioContext.Current.User();
                string pass = "";
                user = HttpUtility.UrlEncode(user);
                pass = HttpUtility.UrlEncode(pass);
                int iProto = url.IndexOf("://");
                if (iProto == -1)
                    iProto = 0;
                else
                    iProto += 3;
                url = url.Insert(iProto, user + ":" + pass + "@");
            }

            Console.WriteLine("Open page: {0}", url);
            this.Driver.Navigate().GoToUrl(url);

            //Auth in IE
            if (isIE && !string.IsNullOrEmpty(ScenarioContext.Current.User()))
            {
                PassAuthenticationIE(ScenarioContext.Current.User(), "");
                this.Driver.SwitchTo().Window(handle);
            }
            InitElements();
            return this;
        }

        private bool checkIsIE(IWebDriver driver)
        {
            bool isIE = driver is InternetExplorerDriver;
            if (!isIE)
            {
                isIE |= ((string)((RemoteWebDriver)driver).Capabilities.GetCapability("BrowserName")) == "InternetExplorer";
            }
            return isIE;
        }

        public void PassAuthenticationIE(string login, string pass)
        {
            try
            {
                TimeSpan pageTimeout = GetPageTimeoutSpan();
                if (pageTimeout < TimeSpan.FromSeconds(5))
                    pageTimeout = TimeSpan.FromSeconds(5);
                while (true)
                {
                    WebDriverWait wait = new WebDriverWait(this.Driver, pageTimeout);
                    IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
                    alert.SetAuthenticationCredentials(login, pass);
                    alert.Accept();
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (NoAlertPresentException) { }
            catch (WebDriverTimeoutException) { }
        }

        public virtual PageCore Refresh()
        {
            this.Driver.Navigate().Refresh();
            return this;
        }

        public void InitElements()
        {
            // init start url
            object[] startUrlAttrs = this.GetType().GetCustomAttributes(typeof(StartUrlAttribute), true);
            if (startUrlAttrs != null && startUrlAttrs.Length > 0)
            {
                this.StartUrl = Alias.ResolveAsString(((StartUrlAttribute)startUrlAttrs[0]).PageAlias);
            }

            TimeSpan classWaitSpan = GetPageTimeoutSpan();

            // init properties
            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                if (!prop.CanWrite)
                    continue;

                object[] findAsAttrs = prop.GetCustomAttributes(typeof(FindAsAttribute), true);

                if (findAsAttrs == null || findAsAttrs.Length == 0)
                    continue;
                FindAsAttribute findAsAttr = (FindAsAttribute)findAsAttrs[0];

                Type elementType = findAsAttr.ElementType;

                if (elementType == null)
                    elementType = prop.PropertyType;

                PageElementCore element = FindElement(findAsAttr.XPath, elementType);

                object[] waitAttrs = prop.GetCustomAttributes(typeof(WaitAttribute), true);

                if (waitAttrs != null && waitAttrs.Length > 0)
                {
                    element.WaitTimeout = ((WaitAttribute)waitAttrs[0]).Timeout;
                }
                else
                {
                    element.WaitTimeout = classWaitSpan;
                }
                
                prop.GetSetMethod().Invoke(this, new object[] { element });                
            }
        }

        public TimeSpan GetPageTimeoutSpan()
        {
            object[] classWaitAttrs = this.GetType().GetCustomAttributes(typeof(WaitAttribute), true);
            if (classWaitAttrs != null && classWaitAttrs.Length > 0)
                return ((WaitAttribute)classWaitAttrs[0]).Timeout;
            return ScenarioContext.Current.WaitTimeout();
        }

        public PageElementCore FindElement(string xPath, Type elementType)
        {
            if (!typeof(PageElementCore).IsAssignableFrom(elementType))
            {
                throw new ArgumentException(string.Format("Тип {0} должен быть унаследован или иметь возможность приведения к {1}",
                    elementType.Name,
                    typeof(PageElementCore).Name));
            }

            return Activator.CreateInstance(elementType, this, xPath) as PageElementCore;
        }

        public PageElement FindElement(string xPath)
        {
            return this.FindElement<PageElement>(xPath);
        }

        public PageElementCollection FindElementCollection(string xPath)
        {
            return this.FindElement<PageElementCollection>(xPath);
        }

        public IWebElement FindWebElement(string xPath, TimeSpan waitTimeout)
        {
            ICollection<IWebElement> elements = this.FindWebElements(xPath, waitTimeout);

            if (elements.Count == 0)
                throw new NoSuchElementException(string.Format("Не найдено ни одного элемента '{0}' (timeout: {1})", xPath, waitTimeout));
            
            return elements.First();
        }

        public ICollection<IWebElement> FindWebElements(string xPath, TimeSpan waitTimeout)
        {
            if (waitTimeout.TotalMilliseconds == 0)
                return this.Driver.FindElements(By.XPath(xPath));

            WebDriverWait wait = new WebDriverWait(this.Driver, waitTimeout);
            return wait.Until(_ => this.Driver.FindElements(By.XPath(xPath)));
        }

        public T FindElement<T>(string xPath) where T: PageElementCore
        {                                                                 
            return Activator.CreateInstance(typeof(T), this, xPath) as T;
        }

        public virtual string Title
        {
            get { return this.Driver.Title; }
        }

        public virtual string Url
        {
            get { return this.Driver.Url; }
            set { this.Driver.Url = value; }
        }

        public virtual string WindowHandle
        {
            get { return this.Driver.CurrentWindowHandle; }
        }

        public bool TryFindElementByAlias(string alias, out PageElementCore element)
        {
            element = null;

            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                if (!prop.HasAlias(alias))
                    continue;

                element = prop.GetGetMethod().Invoke(this, null) as PageElementCore;

                if (element != null)
                    return true;
            }

            return false;
        }

        public void SetWindowSize(int width, int height)
        {
            this.Driver.Manage().Window.Size = new System.Drawing.Size(width, height);
        }

        public void MoveWindowTo(int x, int y)
        {
            this.Driver.Manage().Window.Position = new System.Drawing.Point(x, y);
        }

        public void MaximizeWindow()
        {
            this.Driver.Manage().Window.Maximize();
        }

    
        public static PageCore TryResolveAsPage(string pageAlias)
        {          
                PageCore page = null;

                Type pageType = null;
                if (!Alias.PageAliases.TryGetValue(pageAlias, out pageType))
                    return null;
                page = Activator.CreateInstance(pageType) as PageCore;
                return page;     
        }
    }
}
