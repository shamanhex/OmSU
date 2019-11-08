using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using ITSK.Selenium.PageObjectModel;
using ITSK.UAT.Aliasing;
using System.Threading;

namespace UAT.Terra.Actions.Web
{
    [Binding]
    public static class WebGeneralActions
    {
        [StepDefinition(@"максимальное время загрузки элементов не должно быть больше (.*) сек.")]
        public static void SetWaitTimeout(double sec)
        {
            ScenarioContext.Current.SetWaitTimeout(TimeSpan.FromSeconds(sec));
        }

        [StepDefinition(@"используются следующие псевдонимы:")]
        public static void SetAliases(Table table)
        {
            foreach (TableRow row in table.Rows)
            {
                string alias = row[0];
                string value = row[1];

                Alias.Set(alias, value);
            }
        }

        [StepDefinition(@"адрес сервера ""(.*)""")]
        public static void SetServerUrl(string serverAlias)
        {
            string serverUrl = Alias.ResolveAsString(serverAlias);
            ScenarioContext.Current.SetServerUrl(serverUrl);
        }

        [StepDefinition(@"пользователь нажал ОК в появившемся диалоге")]
        public static void AgreeWithConfirm()
        {
            PageCore page = ScenarioContext.Current.Page();
            page.Driver.SwitchTo().Alert().Accept();
        }

        [StepDefinition("подождал (.*) сек.")]
        public static void WaitForFewSeconds(double sec)
        {
            Thread.Sleep((int)(sec * 1000));
        }
    }
}
