using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITSK.Selenium.PageObjectModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StartUrlAttribute : Attribute
    {
        public string PageAlias { get; private set; }

        public StartUrlAttribute(string pageAlias)
        {
            this.PageAlias = pageAlias;
        }
    }
}
