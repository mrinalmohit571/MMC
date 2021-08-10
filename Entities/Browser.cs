using MMC.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;

namespace Entities
{
    [XmlRoot(ElementName = "Browser")]
    public class Browser
    {
        [XmlElement(ElementName = "DriverPath")]
        public string DriverPath { get; set; }
        [XmlElement(ElementName = "DriverCapabilities")]
        public string DriverCapabilities { get; set; }
        [XmlElement(ElementName = "UseDefaultProfile")]
        public string UseDefaultProfile { get; set; }
        [XmlElement(ElementName = "PathToProfile")]
        public string PathToProfile { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        public static ThreadLocal<IWebDriver> _driver = new ThreadLocal<IWebDriver>(true);
        public static WebDriverWait wait;
        public static IWebDriver Get(BrowserType browserType)
        {
            //if driver is not initialized
            if (!HasWebDriverStarted())
            {
                Init(browserType);
            }

            return _driver.Value;
        }

        private static bool HasWebDriverStarted()
        {
            return _driver.IsValueCreated;

        }

        public static void Close()
        {
           if( _driver.IsValueCreated)
            _driver.Value.Quit();
            
        }

        private static void Init(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    {
                       
                        System.Environment.SetEnvironmentVariable("webdriver.chrome.driver", ".\\MMC\\MMC\\chromedriver");
                        _driver.Value = new ChromeDriver();
                        _driver.Value.Manage().Timeouts().ImplicitWait = FrameworkConstants.ShortTimeSpan;

                        break;
                    }
                case BrowserType.FireFox:
                    {                        
                        _driver.Value = new FirefoxDriver();
                        _driver.Value.Manage().Timeouts().ImplicitWait = FrameworkConstants.ShortTimeSpan;

                        break;
                    }
                default:
                    throw new NotSupportedException(browserType + "is not supported");
            }
        }

        public static void WaitForLoad(BrowserType browserType = BrowserType.Chrome, TimeSpan? timeSpan = null)
        {
            timeSpan = timeSpan ?? FrameworkConstants.LongTimeSpan;

            var javaScriptExecutor = (IJavaScriptExecutor)Browser.Get(browserType);
            wait = new WebDriverWait(Browser.Get(browserType), (TimeSpan)timeSpan);
            wait.Until(wd => javaScriptExecutor.ExecuteScript("return document.readyState").ToString() == "complete");
        }


    }
    [XmlRoot(ElementName = "Browsers")]
    public class Browsers
    {
        [XmlElement(ElementName = "Browser")]
        public List<Browser> Browser { get; set; }
    }

}
