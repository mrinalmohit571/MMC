using MMC.Utils;
using Entities;
using System;
using System.Drawing;

namespace MMC
{
    public class UXBase : StepBase
    {

        public new bool NavigateToURL(string URL)
        {
            bool IsNavigationSuccess = false;
            try
            {eyes.Open(Browser.Get(browserType), "AppName", "Test1", new Size(1024, 800));
                Browser.Get(browserType).Url = URL;
            }
            catch (Exception ex)
            {
                Logger.LogError("Navigation to URL failed" + ex.Message);
                throw new Exception("Navigation to URL failed" + ex.Message);
            }
            return IsNavigationSuccess;
        }
        
    }
}
