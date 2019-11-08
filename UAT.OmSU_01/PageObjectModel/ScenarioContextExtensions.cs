using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITSK.AssemblyAllureAdapter.Attributes;
using TechTalk.SpecFlow;

namespace ITSK.Selenium.PageObjectModel
{
    public static class ScenarioContextExtensions
    {
        public const string CURRENT_PAGE_KEY = "CurrentPage";
        public const string CURRENT_ELEMENT_KEY = "CurrentElement";
        public const string CURRENT_ELEMENT_GROUP_KEY = "CurrentElementGroup";
        public const string USER_ELEMENT_KEY = "CurrentUserName";
        public const string PASSWORD_ELEMENT_KEY = "CurrentUserPassword";
        public const string WAIT_TIMEOUT_KEY = "WaitTimeout";

        //[AAAStep("TryResolveAsPage")]
        public static void SetPage(this ScenarioContext context, PageCore page)
        {
            context[CURRENT_PAGE_KEY] = page;
        }

        public static T Page<T>(this ScenarioContext context) where T : PageCore
        {
            T value;
            if (context.TryGetValue<T>(CURRENT_PAGE_KEY, out value))
                return value;
            else
            {
                return null;
            }
        }

        public static void SetCurrent(this ScenarioContext context, PageElementCore elementCore)
        {
            if (elementCore is PageElement)
                context.SetElement(elementCore as PageElement);
            else if (elementCore is PageElementCollection)
                context.SetElementGroup(elementCore as PageElementCollection);
        }

        public static PageCore Page(this ScenarioContext context)
        {
            return context.Page<PageCore>();
        }

        public static void SetElement(this ScenarioContext context, PageElement element)
        {
            context[CURRENT_ELEMENT_KEY] = element;
            context.Remove(CURRENT_ELEMENT_GROUP_KEY);
        }
           
        public static T Element<T>(this ScenarioContext context) where T : PageElement
        {
            T value;
            if (context.TryGetValue<T>(CURRENT_ELEMENT_KEY, out value))
                return value;
            else
                return null;
        }

        public static PageElement Element(this ScenarioContext context)
        {
            return context.Element<PageElement>();
        }

        public static PageElementCollection ElementGroup(this ScenarioContext context)
        {
            PageElementCollection value;
            if (context.TryGetValue<PageElementCollection>(CURRENT_ELEMENT_GROUP_KEY, out value))
                return value;
            else
                return null;
        }

        public static void SetElementGroup(this ScenarioContext context, PageElementCollection group)
        {
            context[CURRENT_ELEMENT_GROUP_KEY] = group;
            context.Remove(CURRENT_ELEMENT_KEY);
        }

        public static void SetUser(this ScenarioContext context, string userName, string password)
        {
            context[USER_ELEMENT_KEY] = userName;
            context[PASSWORD_ELEMENT_KEY] = password;
        }

        public static string User(this ScenarioContext context)
        {
            object user = null;
            if (context.TryGetValue(USER_ELEMENT_KEY, out user))
                return Convert.ToString(user);
            else
                return null;
        }
        
        public static void SetWaitTimeout(this ScenarioContext context, TimeSpan timeout)
        {
            ScenarioContext.Current[WAIT_TIMEOUT_KEY] = timeout;
        }

        public static TimeSpan WaitTimeout(this ScenarioContext context)
        {
            TimeSpan timeout;
            if (context.TryGetValue(WAIT_TIMEOUT_KEY, out timeout))
                return timeout;
            return TimeSpan.Zero;
        }

    }
}
