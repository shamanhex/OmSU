using ITSK.UAT.Aliasing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using System.Drawing;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace ITSK.Selenium.PageObjectModel
{
    public class PageElement : PageElementCore
    {
        public virtual IWebElement Element
        {
            get
            {
                if (this._element == null)
                {
                    try
                    {
                        this._element = this.Page.FindWebElement(this.XPath, this.WaitTimeout);
                    }
                    catch (WebDriverTimeoutException ex)
                    {
                        throw new InvalidOperationException(string.Format("Элемент не найден или не появился в течении заданного времени (xPath: {0}). Ошибка: {1}", XPath, ex.Message), ex);
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
                return this._element;
            }
            set
            {
                this._element = value;
            }
        }
        
        private IWebElement _element = null;

        public PageElement(PageCore page, string xPath) : base(page, xPath) { }

        public override bool IsDisplayed
        {
            get
            {
                try
                {
                    return this.Element.Displayed;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
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

        public virtual void Clear()
        {
            this.Element.Clear();
        }

        public virtual void TypeText(string text)
        {
            this.Element.SendKeys(text); 
        }

        public virtual void PressEnter()
        {
            this.Element.SendKeys(Keys.Enter);
        }

        public virtual void PressTab()
        {
            this.Element.SendKeys(Keys.Tab);
        }

        public virtual void Click()
        {
            //this.MouseDown(0,0);
            //this.MouseUp(0,0);
            this.Element.Click();
            
        }
        
        //******************************************
        //public virtual void MoveMouseTo()
        //{
        //    Actions action = new Actions(this.Page.Driver);
        //    action.MoveToElement(this.Element).Build().Perform();
        //}
        //******************************************

        public virtual void MoveMouseTo(int centerOffsetX, int centerOffsetY)
        {
            IHasInputDevices device = this.Page.Driver as IHasInputDevices;

            ICoordinates point = new Coordinates(this.CenterCoordinates, centerOffsetX, centerOffsetY);
            if (point == null)
            {
                throw new InvalidOperationException(string.Format("Невозможно получить координаты элемента (XPath: {0}) для имитации события MouseUp.", this.XPath));
            }            

            Size size = this.Element.Size;
            Console.WriteLine("Имитация MouseMove() в [{0};{1}]", point.LocationInDom.X, point.LocationInDom.Y);

            ILocatable hoverItem = (ILocatable)this.Page.Driver.FindElement(By.XPath(this.XPath));
            if (hoverItem == null)
            {
                throw new InvalidOperationException(string.Format("Невозможно получить координаты элемента (XPath: {0}) для имитации события MouseUp.", this.XPath));
            }

            device.Mouse.MouseMove(hoverItem.Coordinates);
            //device.Mouse.MouseMove(point, 0, 0);
        }

        public virtual void MouseDown(int centerOffsetX, int centerOffsetY)
        {
            IHasInputDevices device = this.Page.Driver as IHasInputDevices;

            ICoordinates point = new Coordinates(this.CenterCoordinates, centerOffsetX, centerOffsetY);
            if (point == null)
            {
                throw new InvalidOperationException(string.Format("Невозможно получить координаты элемента (XPath: {0}) для имитации события MouseUp.", this.XPath));
            }

            Console.WriteLine("Имитация MouseDown() в [{0};{1}]", point.LocationInDom.X, point.LocationInDom.Y);

            device.Mouse.MouseDown(point);
        }

        public virtual void MouseUp(int centerOffsetX, int centerOffsetY)
        {
            IHasInputDevices device = this.Page.Driver as IHasInputDevices;

            ICoordinates point = new Coordinates(this.CenterCoordinates, centerOffsetX, centerOffsetY);
            if (point == null)
            {
                throw new InvalidOperationException(string.Format("Невозможно получить координаты элемента (XPath: {0}) для имитации события MouseUp.", this.XPath));
            }

            Console.WriteLine("Имитация MouseUp() в [{0};{1}]", point.LocationInDom.X, point.LocationInDom.Y);

            device.Mouse.MouseUp(point);                
        }
        
        public virtual string Text
        {
            get
            {
                return this.Element.Text;
            }
        }

        Regex regex = new Regex(@"\s\s+", RegexOptions.Compiled);

        public virtual string TextAsLine
        {
            get
            {
                string str = this.Text;
                str = str.Replace('\r', ' ').Replace('\n', ' ');
                str = regex.Replace(str, " ");
                return str;
            }
        }

        public ICoordinates Coordinates
        {
            get
            {
                IHasInputDevices device = this.Page.Driver as IHasInputDevices;

                if (device == null)
                {
                    return null;
                }

                ICoordinates coordinates = this.Element as ICoordinates;
                    
                if (coordinates != null)
                {
                    return coordinates;
                }

                PropertyInfo coordinatesProperty = this.Element.GetType().GetProperty("Coordinates", typeof(ICoordinates));

                if (coordinatesProperty == null)
                {
                    return null;
                }

                object value = coordinatesProperty.GetGetMethod().Invoke(this.Element, null);
                return value as ICoordinates;
            }
        }

        public ICoordinates CenterCoordinates
        {
            get
            {
                Size size = this.Element.Size;
                int offsetX = size.Width / 2;
                int offsetY = size.Height / 2;

                Coordinates coordinates = new Coordinates(this.Coordinates, offsetX, offsetY);
                
                return coordinates;
            }
        }
    }
}
