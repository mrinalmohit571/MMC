using MMC.Entities;
using MMC.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace MMC.StepDefinition
{
    [Binding]
    public sealed class UIStepDefinition : StepBase
    {
        
        private readonly ScenarioContext _scenarioContext;

        public UIStepDefinition(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"User navigate to (.*) Search Page")]
        public void GivenUserNavigateToGoogleSearchPage(string AppName)
        {
            LoadApplication(AppName);
            NavigateToURL(Helper.GetAppURLBasedonEnv(AppName));
            bool isNavigationSuccess = Page("GoogleSearch").VerifyTitle("Google");
            Assert.AreEqual(true, isNavigationSuccess, "Navigation to URL failed");
        }

        [Given(@"Enter (.*) in the search box")]
        public void GivenEnterTextInTheSearchBox(String text)
        {
            Item SearchData = GetSearchData(text, "searchData");
            Page("GoogleSearch").SendKeysToElement("txtSearchBox", SearchData.Title);
        }

        [When(@"Click the search button")]
        public void WhenClickTheSearchButton()
        {
            Page("GoogleSearch").SendKeysToElement("txtSearchBox", Keys.Enter);
        }

        [When(@"Go to the (.*) section")]
        public void WhenGoToGivenSection(String Section)
        {
            WaitForPageToLoad();
            Item SearchData = GetSearchData(Section, "searchData");
            Page("GoogleSearch").VerifyTitle(SearchData.Title + " - Google Search");
            Page("GoogleSearch").ClickDynamicElement("Sectionlnk", SearchData.Section);


        }

        [Then(@"Click on the (.*) result")]
        public void ThenClickOnResult(String Section)
        {
            WaitForPageToLoad();
            Item SearchData = GetSearchData(Section, "searchData");
            Page("GoogleSearch").ClickDynamicElement("BookLink_Index", SearchData.Index);
        }

        [Then(@"Verify it open a correct link")]
        public void ThenVerifyItOpenACorrectLink()
        {
            WaitForPageToLoad();
            Page("GoogleSearch").VerifyTitle("- Google Books");
        }

        [Then(@"verify it open (.*) in google map")]
        public void ThenVerifyItOpenInGoogleMap(String Place)
        {
            WaitForPageToLoad();
            Item SearchData = GetSearchData(Place, "searchData");
            Page("GoogleSearch").VerifyTitle(SearchData.Title + " - Google Maps");
        }


        [Then(@"User Zoom the map out until the map scale reaches the value (.*)")]
        public void ThenUserZoomTheMapOut(String scale)
        {
            Item SearchData = GetSearchData(scale, "searchData");
            int distance = Convert.ToInt32((GetElementText("GoogleMap", "scaleDistance"))
                .Trim().Split(' ')[0]);
            while (distance < Convert.ToInt32(SearchData.Scale))
            {
                Page("GoogleMap").ClickElement("zoomOutBtn");
                distance = Convert.ToInt32((GetElementText("GoogleMap", "scaleDistance"))
                .Trim().Split(' ')[0]);
            }
        }

    }
}
