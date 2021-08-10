using OpenQA.Selenium;
using System;
using System.Threading;

namespace MMC.Utils
{
    public static class Helper
    {
        public static string Capture(IWebDriver driver, string screenShotName)
        {

            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();
            string pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            string finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "ErrorScreenshots\\" + screenShotName + ".png";
            string localpath = new Uri(finalpth).LocalPath;
            screenshot.SaveAsFile(localpath);
            return localpath;
        }

        public static string GetAppURLBasedonEnv(string appName)
        {
            string AppURL = string.Empty;
             string environment = appName.Trim().Split('_')[1];

            try
            {               
                foreach (var application in Hooks.config.Applications.Application)
                {
                    if (application.Name.ToUpper().Trim().Equals(appName.ToUpper().Trim()))
                    {
                        if (environment != null)
                        {
                            if (environment.Contains("UI"))
                            {
                                AppURL = application.UIURL;
                            }
                            else
                             if (environment.Contains("API"))
                            {
                                AppURL = application.APIURL;
                            }
                            else

                            {
                                Logger.LogError(environment + " is not a valid environment");
                                throw new Exception(environment + " is not a valid environment");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Getting error while fetching URL:" + ex.Message);
                throw new Exception("Getting error while fetching URL:" + ex.Message);
            }
            return AppURL;
        }

        public static string UpdateXPathWithOriginalVal(string xpath, string Val)
        {
            string UpdatedXPath = string.Empty;
            UpdatedXPath = xpath.Replace("$Val$", Val);
            return UpdatedXPath;
        }

        internal static void RunInSTA(Action action)
        {
            Thread thread = new Thread(() => action());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        }


}

