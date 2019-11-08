using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITSK.Selenium.PageObjectModel
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class WaitAttribute : Attribute
    {
        public TimeSpan Timeout = TimeSpan.Zero;

        public WaitAttribute(TimeSpan timeout)
        {
            this.Timeout = timeout;
        }

        public WaitAttribute(double seconds)
        {
            this.Timeout = TimeSpan.FromSeconds(seconds);
        }
    }
}
