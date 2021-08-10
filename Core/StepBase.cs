using Applitools.Selenium;
using MMC.Enums;
using MMC.Utils;
using Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using MMC.Entities;
using Logger = MMC.Utils.Logger;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Applitools.VisualGrid;
using System.Configuration;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(3)]

namespace MMC
{
    [Binding]
    public class StepBase : Steps
    {
        private static StepBase instance = null;
        private int msgCount = 0;

        public static Enums.BrowserType browserType = TestContext.Parameters.Get("browser", Enums.BrowserType.Chrome);
        public static StepBase getInstance()
        {

            if (instance == null)
            {
                instance = new StepBase();
            }
            return instance;
        }

        public static WebDriverWait wait;
        public static Eyes eyes;

       
        public string ApplicationName { get; set; }

        public static string PageName { get; set; }

        public Dictionary<string, Locator> LocatorList { get; set; }

        public Dictionary<string, Page> PageList = new Dictionary<string, Page>();

        public void setPageList(Dictionary<string, Page> pageList)
        {
            PageList = pageList;
        }

        public string ErrorMessage { get; set; }

        public StepBase Page(string PageName)
        {
            if (String.IsNullOrEmpty(PageName))
            {
                Logger.LogError("PageName cannot be null or empty");
                throw new Exception("PageName cannot be null or empty");
            }
            StepBase.PageName = PageName;
            return this;
        }

        private static void LoadDriverConfigurationBasedonTestType(string sTestType)
        {
            try
            {
                switch (sTestType)
                {
                    case "UI":
                        Browser.WaitForLoad(browserType);
                        break;
                    case "API":

                       break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Loading Driver for Test Type" + ex);
            }
        }



        /****************************************Framework Level Attributes***********************/


        public void AppendMsgToReport(string msg)
        {
            string key = "Message" + msgCount;
            ScenarioStepContext.Current.Add(key, msg);
            msgCount++;
        }

        public void LoadLocatorList(string applicationName)
        {
            try
            {
                Dictionary<string, Page> pageList = new Dictionary<string, Page>();
                string ORLocation = Hooks.config.getRelativePath(@"XMLs\OR.xml");
                //Get all the locatorvle in the XML for given application
                XElement xelement = XElement.Load(ORLocation);
                IEnumerable<XElement> Applications = xelement.Elements();

                foreach (var app in Applications)
                {
                    if (app.Attribute("name").Value == applicationName)
                    {
                        foreach (var page in app.Elements())
                        {
                            Dictionary<string, Locator> locatorList = new Dictionary<string, Locator>();
                            string PageName = page.Attribute("name").Value;
                            foreach (var locator in page.Elements())
                            {
                                if (string.IsNullOrEmpty(page.Attribute("name").Value) || string.IsNullOrEmpty(locator.Attribute("name").Value) || string.IsNullOrEmpty(locator.Attribute("Value").Value) || string.IsNullOrEmpty(locator.Attribute("Type").Value))
                                {
                                    throw new Exception("Either PageName or Locator items are blank/invalid");
                                }
                                Locator local = new Locator()
                                {
                                    LocatorName = locator.Attribute("name").Value,
                                    LocatorType = (LocatorType)Enum.Parse(typeof(LocatorType), locator.Attribute("Type").Value),
                                    LocatorValue = locator.Attribute("Value").Value
                                };
                                locatorList.Add(locator.Attribute("name").Value, local);
                            }
                            Page pg = new Page()
                            {
                                PageName = PageName,
                                LocatorList = locatorList
                            };
                            pageList.Add(PageName, pg);
                        }
                        break;
                    }
                }
                getInstance().setPageList(pageList);
            }
            catch (Exception ex)
            {
                Logger.LogError("Locators are not loaded properly for application:" + applicationName + ".Exception:" + ex);
                throw new Exception("Locators are not loaded properly for application:" + applicationName);
            }
        }

        public void LoadApplication(string AppName)
        {
            try
            {
                ApplicationName = AppName;
                LoadLocatorList(ApplicationName);
            }
            catch (Exception ex)
            {
                Logger.LogError("Unable to load locators.Error Occured is:" + ex.Message);
                throw new Exception("Unable to load locators.Error Occured is:" + ex.Message);
            }
            
            LoadDriverConfigurationBasedonTestType(AppName.Trim().Split('_')[1]);
        }

        public void WaitForPageToLoad()
        {
            wait = new WebDriverWait(Browser.Get(browserType), TimeSpan.FromSeconds(10));
            TimeSpan timeout = new TimeSpan(0, 0, 30);

            IJavaScriptExecutor javascript = (IJavaScriptExecutor)Browser.Get(browserType);
            if (javascript == null)
                throw new ArgumentException("driver", "Driver must support javascript execution");

            wait.Until((d) =>
            {
                try
                {
                    string readyState = javascript.ExecuteScript(
                    "if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available
                    return e.Message.ToLower().Contains("unable to get browser");
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public IWebElement GetDynamicElement(string elementName, string valuetoreplace)
        {
            IWebElement element = null;
            try
            {
                string LocatorValue = Helper.UpdateXPathWithOriginalVal(StepBase.getInstance().PageList[PageName].LocatorList[elementName].LocatorValue, valuetoreplace);
                element = FindElement(StepBase.getInstance().PageList[PageName].LocatorList[elementName].LocatorType, LocatorValue);
            }
            catch (NoSuchElementException ex)
            {
                Logger.LogError("Unable to find element:" + elementName + " Exception:" + ex);
                throw new Exception("Unable to Find Element: " + elementName);
            }
            catch (ElementNotSelectableException ex)
            {
                Logger.LogError("Unable to select element: " + elementName + " Exception:" + ex);
                throw new Exception("Unable to select element: " + elementName);
            }
            catch (ElementNotVisibleException ex)
            {
                Logger.LogError("Element: " + elementName + " is not visible.Exception:" + ex);
                throw new Exception("Element: " + elementName + " is not visible.");
            }
            catch (TimeoutException ex)
            {
                Logger.LogError("Timeout Issue while finding element: " + elementName + "Exception:" + ex);
                throw new Exception("Timeout Issue while finding element: " + elementName);
            }
            catch (StaleElementReferenceException ex)
            {
                Logger.LogError("Stale Element Exceltion occured for element: " + elementName + "Exception:" + ex);
                throw new Exception("Stale Element Exceltion occured for element: " + elementName);
            }
            catch (Exception ex)
            {
                Logger.LogError("Unable to find element: " + elementName + "Exception:" + ex);
                throw new Exception("Unable to find element: " + elementName + "Exception:" + ex);
            }
            return element;
        }

        public IWebElement GetElement(string elementName)
        {
            IWebElement element = null;
            try
            {
                element = FindElement(StepBase.getInstance().PageList[PageName].LocatorList[elementName].LocatorType, StepBase.getInstance().PageList[PageName].LocatorList[elementName].LocatorValue);               
            }
            catch (NoSuchElementException ex)
            {
                Logger.LogError("Unable to find element:" + elementName + "Exception:" + ex);
                throw new Exception("Unable to Find Element:" + elementName);
            }
            catch (ElementNotSelectableException ex)
            {
                Logger.LogError("Unable to select element:" + elementName + "Exception:" + ex);
                throw new Exception("Unable to select element:" + elementName);
            }
            catch (ElementNotVisibleException ex)
            {
                Logger.LogError("Element is not visible:" + elementName + "Exception:" + ex);
                throw new Exception("Element is not visible:" + elementName);
            }
            catch (TimeoutException ex)
            {
                Logger.LogError("Timeout Issue while finding element:" + elementName + "Exception:" + ex);
                throw new Exception("Timeout Issue while finding element:" + elementName);
            }
            catch (StaleElementReferenceException ex)
            {
                Logger.LogError("Stale Element Exceltion occured for element:" + elementName + "Exception:" + ex);
                throw new Exception("Stale Element Exceltion occured for element:" + elementName);
            }
            catch (Exception ex)
            {
                Logger.LogError("Unable to find element:" + elementName + "Exception:" + ex);
                throw new Exception("Unable to find element:" + elementName + "Exception:" + ex);
            }

            return element;
        }


        protected IWebElement FindElement(LocatorType locatorType, string locatorValue)
        {
            IWebElement ele = null;
            try
            {                
                switch (locatorType)
                {
                    case LocatorType.XPath:
                        if (IsElementPresent(By.XPath(locatorValue)))
                        {
                            ele = Browser.Get(browserType).FindElement(By.XPath(locatorValue));
                        }

                        break;
                    case LocatorType.ID:
                        if (IsElementPresent(By.Id(locatorValue)))
                        {
                            ele = Browser.Get(browserType).FindElement(By.Id(locatorValue));
                        }

                        break;
                    case LocatorType.TagName:
                        if (IsElementPresent(By.TagName(locatorValue)))
                        {
                            ele = Browser.Get(browserType).FindElement(By.TagName(locatorValue));
                        }

                        break;
                    case LocatorType.Name:
                        if (IsElementPresent(By.Name(locatorValue)))
                        {
                            ele = Browser.Get(browserType).FindElement(By.Name(locatorValue));
                        }

                        break;
                    case LocatorType.ClassName:
                        if (IsElementPresent(By.ClassName(locatorValue)))
                        {
                            ele = Browser.Get(browserType).FindElement(By.ClassName(locatorValue));
                        }

                        break;
                    case LocatorType.LinkText:
                        if (IsElementPresent(By.LinkText(locatorValue)))
                        {
                            ele = Browser.Get(browserType).FindElement(By.LinkText(locatorValue));
                        }

                        break;
                    case LocatorType.CssSelector:
                        if (IsElementPresent(By.CssSelector(locatorValue)))
                        {
                            ele = Browser.Get(browserType).FindElement(By.CssSelector(locatorValue));
                        }

                        break;
                }
            }
            catch (NoSuchElementException ex)
            {
                Logger.LogError("Unable to find element:" + ele + "Exception:" + ex);
                throw new Exception("Unable to Find Element:" + ele);
            }
            return ele;
        }


        private bool IsElementPresent(By by)
        {
            WebDriverWait wait = new WebDriverWait(Browser.Get(browserType), TimeSpan.FromSeconds(60));
            try
            {
                wait.Until(ExpectedConditions.ElementExists(by));
                return Browser.Get(browserType).FindElement(by).Displayed;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool NavigateToURL(string URL)
        {
            bool IsNavigationSuccess = false;
            try
            {
                wait = Browser.wait;
                Browser.Get(browserType).Navigate().GoToUrl(URL);
                Logger.LogInfo("Navigating to URL:" + URL);
                Browser.Get(browserType).Manage().Window.Maximize();
                Browser.Get(browserType).Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                VerifyPageUrl(URL);
                IsNavigationSuccess = true;
            }
            catch (Exception e)
            {
                Logger.LogError("Navigation to URL " + URL + " failed due to " + e.Message);
                throw new Exception("Navigation to URL " + URL + " failed due to " + e.Message);
            }
            return IsNavigationSuccess;
        }

        public bool VerifyPageUrl(string URL)
        {
            return Browser.Get(browserType).Url.Contains(URL);

        }

        public bool ClickDynamicElement(string elementName, string ValToUpdate)
        {
            bool Clicked = false;
            try
            {
                IWebElement element = GetDynamicElement(elementName, ValToUpdate);
                element.Click();
                Clicked = true;
            }
            catch (ElementNotVisibleException ex)
            {
                Logger.LogError("Element:" + elementName + " is not visible on the page.Exception:" + ex.Message);
                throw new Exception("Element:" + elementName + " is not visible on the page");
            }
            catch (StaleElementReferenceException ex)
            {
                Logger.LogError("Element:" + elementName + " is no longer valid on the page.Exception:" + ex.Message);
                throw new Exception("Element:" + elementName + " is no longer valid on the page");
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception while clicking dynamic element:" + ex.Message);
                throw new Exception("Exception while clicking dynamic element:" + ex.ToString());
            }
            return Clicked;
        }


        public bool ClickElement(string elementName)
        {
            bool Clicked = false;
            try
            {
                IWebElement El = GetElement(elementName);
                new WebDriverWait(Browser.Get(browserType), TimeSpan.FromSeconds(60)).Until(ExpectedConditions.ElementToBeClickable(El));
                El.Click();

                Clicked = true;
                Thread.Sleep(4000);

            }
            catch (ElementClickInterceptedException Iex)
            {
                ClickToElementByJS(elementName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return Clicked;
        }


        public void SendKeysToElement(string elementName, string text)
        {
            wait = new WebDriverWait(Browser.Get(browserType), TimeSpan.FromSeconds(20));
            try
            {
                IWebElement El = GetElement(elementName);
                wait.Until(ExpectedConditions.ElementToBeClickable(El)).SendKeys(text);
                Thread.Sleep(4000);
                WaitForPageToLoad();
            }
            catch (Exception ex)
            {
                Logger.LogError("Issue while inputting '" + text + "' in element:" + elementName + " .Error occured is:" + ex);
                throw new Exception("Issue while inputting '" + text + "' in element:" + elementName);
            }
        }

        public void ClickToElementByJS(string elementName)
        {
            IWebElement element = GetElement(elementName);
            var javaScriptExecutor = (IJavaScriptExecutor)Browser.Get(browserType);
            javaScriptExecutor.ExecuteScript("arguments[0].click();", element);
        }


        public bool VerifyTitle(string ExpTitle)
        {
            bool titleMatched = false;
            try
            {
                if (Browser.Get(browserType).Title.Contains(ExpTitle))
                {
                    titleMatched = true;
                }
                Assert.IsTrue(titleMatched);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unable to verify title: Exception" + ex.Message);
            }

            return titleMatched;
        }

        public string GetElementText(string pageName, string elementName)
        {
            string text;
            try
            {
                Page(pageName);
                IWebElement El = GetElement(elementName);
                text = El.Text.Trim();

            }
            catch (Exception ex)
            {
                Logger.LogError("Issue while fetching text of element:" + elementName + ".Error occured is:" + ex.Message);
                throw new Exception("Issue while fetching text of element:" + elementName + ".Error occured is:" + ex.Message);
            }
            return text;
        }
         

        public Entities.Item GetSearchData(string OrderPSID, String FileName)
        {
            bool SearchDataFound = false;
            SearchData obj = new SearchData();
            try
            {
                Entities.SearchData SearchData = (Entities.SearchData)XMLUtils.LoadXMLObject(obj, FileName+".xml");

                foreach (var data in SearchData.Item)
                {
                    if (String.Equals(data.Name, OrderPSID))
                    {
                        SearchDataFound = true;
                        return data;
                    }

                };
                if (SearchDataFound == false)
                {
                    Assert.Fail("Invalid Order details: " + SearchDataFound);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Search data not retreived");
                throw new Exception("Search data not retreived ", e);
            }
            return null;
        }

    }
}
