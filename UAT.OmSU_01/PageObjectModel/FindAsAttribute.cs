using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITSK.Selenium.PageObjectModel
{
    [AttributeUsage(AttributeTargets.Property, 
                    AllowMultiple = false, 
                    Inherited = true)]
    public class FindAsAttribute : Attribute
    {
        public string XPath = null;
        public Type ElementType = null;

        public FindAsAttribute(string xPath)
        {
            this.XPath = xPath;
            this.ElementType = null;
        }

        public FindAsAttribute(string xPath, Type elementType)
        {
            if (!(typeof(PageElementCore).IsAssignableFrom(elementType)))
                throw new ArgumentException(string.Format("Тип {0} невозможно привести к типу {1}.", elementType, typeof(PageElementCore)), "elementType");

            this.XPath = xPath;
            this.ElementType = elementType;
        }
    }
}
