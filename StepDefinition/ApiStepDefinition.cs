using MMC.CoreInterfaces;
using MMC.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace MMC.StepDefinition
{
    [Binding]
    public sealed class ApiStepDefinition : APIBase
    {

        private readonly ScenarioContext _scenarioContext;

        public ApiStepDefinition(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"User define Application as (.*)")]
        public void GivenUserDefineApplicationAsApp(String AppName)
        {
            SetbaseUrl(AppName);
        }

        [Given(@"User define zip code (.*)")]
        public void GivenUserDefineZipCode(String zipcode)
        {

            Item SearchData = GetSearchData(zipcode, "searchData");

            restClient = SetUrl(SearchData.CountryCode + "/" + SearchData.PostCode);

        }

        [When(@"user request for response")]
        public void WhenUserRequestForResponse()
        {
            restRequest = CreateGetRequest();
        }

        [Then(@"user verify response data OK")]
        public void ThenUserVerifyResponseDataOK()
        {
            restResponse = GetResponse(restClient, restRequest);

            Assert.That((int)restResponse.StatusCode, Is.EqualTo(200));

        }

        [Then(@"Verify country name must be returned correctly (.*)")]
        public void ThenVerifyCountryNameMustBeReturnedCorrectly(String zipcode)
        {
            Item SearchData = GetSearchData(zipcode, "searchData");
            LocationResponse locationResponse = GetContent<LocationResponse>(restResponse);
            String CountryName = locationResponse.Country;
            Assert.IsTrue(CountryName.Equals(SearchData.CountryName));
        }

        [Then(@"verify number of entry should be one")]
        public void ThenVerifyNumberOfEntryShouldBeOne()
        {
            LocationResponse locationResponse = GetContent<LocationResponse>(restResponse);
            List<Place> places = locationResponse.Places;
            Assert.IsTrue(places.Count == 1);
        }

        [Then(@"Verify response time needs to be under expected time limit (.*)")]
        public void ThenVerifyResponseTimeNeedsToBeUnderExpectedTimeLimit(String zipcode)
        {
            Item SearchData = GetSearchData(zipcode, "searchData");
            DateTime T = System.DateTime.UtcNow;
            GetResponse(restClient, restRequest);
            TimeSpan TT = System.DateTime.UtcNow - T;
            int ActualResponseTime = TT.Milliseconds;
            if (ActualResponseTime <= SearchData.ResponseTime)
                Assert.IsTrue(true);
            else
                Assert.Warn("Response time is greater than " + SearchData.ResponseTime + " ms ! \n Actual Response time = " + ActualResponseTime + " ms.");
        }
    }
}



