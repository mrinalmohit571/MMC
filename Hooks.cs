using Entities;
using MMC.Utils;
using OpenQA.Selenium;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;
using Logger = MMC.Utils.Logger;

namespace MMC
{
    [Binding]
    public class Hooks
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private static readonly ScenarioContext _scenarioContext;
        private static readonly FeatureContext featureContext;

        public static Config config;
        public static ReportWriter reportWriter { get; set; }
        private static bool featureResult;
        private static DataTable dt;
        private static DataRow dr;
        public static string TestType;
        public static bool ExtentReportFlag = false;

        /// <summary>
        /// 
        /// </summary>
        [BeforeTestRun]
        public static void InitializeFramework()
        {
            config = new Config();
            config = config.LoadConfiguration();



            if (config.ReportType == "Extent")
            {
                ExtentReportFlag = true;
                try
                {
                    System.IO.Directory.CreateDirectory(XMLUtils.GetRelativePath(config.LogsLocation));

                    // Set the Logger for the Project 
                    string LogName = config.LogsLocation;
                    log4net.GlobalContext.Properties["LogName"] = LogName + "\\" + "TestPlanLog.log";

                    // Set the Reporting Engine 
                    string _reportName = config.ReportLocation + "\\" + DateTime.Now.ToString("yyyyMMdd HHmm");
                    string _reportPath = config.getRelativePath(_reportName);


                    System.IO.Directory.CreateDirectory(_reportPath);
                    reportWriter = new ReportWriter(_reportPath);

                    dt = new DataTable();
                    dt.Columns.Add("Plan");
                    dt.Columns.Add("Test Case");
                    dt.Columns.Add("Step");
                    dt.Columns.Add("Status");
                    Logger.LogInfo(string.Format("SUITE EXECUTION HAS BEEN STARTED."));
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in initializing framework" + ex.Message);
                    throw new Exception("Error in initializing framework" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [AfterTestRun]
        public static void TearDownReport()
        {
            if (ExtentReportFlag)
            {
                reportWriter.SaveReport();
            }
            Logger.LogInfo(string.Format("SUITE EXECUTION HAS BEEN ENDED."));
        }

        [BeforeFeature]
        public static void LoadDriver(FeatureContext featureContext)
        {
            if (ExtentReportFlag)
            {
                reportWriter.StartTestPlan(featureContext.FeatureInfo.Title, "Description for " + featureContext.FeatureInfo.Title);
            }
            featureResult = true;
        }

        [BeforeScenario]
        public static void InitApplication(FeatureContext featureContext, ScenarioContext scenarioContext)
        {

            Logger.LogInfo(string.Format("TEST PLAN - {0} EXECUTION HAS BEEN STARTED.", featureContext.FeatureInfo.Title));
            if (ExtentReportFlag)
            {
                reportWriter.StartTestCase(featureContext.FeatureInfo.Title, scenarioContext.ScenarioInfo.Title, "Description for " + scenarioContext.ScenarioInfo.Title);
            }
        }

        [AfterStep]
        public static void WriteReport(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            var errormessage = "";
            if (ExtentReportFlag)
            {

                try
                {
                    string message = "";

                    if (scenarioContext.StepContext.ContainsKey("Message"))
                    {
                        message = scenarioContext.StepContext["Message"].ToString();
                    }
                    if (scenarioContext.TestError != null)
                    {
                        var error = scenarioContext.TestError;
                        errormessage = "<pre>" + error.Message + "</pre>";
                        if (((NUnit.Framework.ResultStateException)scenarioContext.TestError).ResultState.Status == NUnit.Framework.Interfaces.TestStatus.Passed)
                        {
                            reportWriter.PassTestCase(ReportStepBody(scenarioContext, true, errormessage));

                        }
                        else
                        {
                            featureResult = false;
                            reportWriter.FailTestCase(ReportStepBody(scenarioContext, false, errormessage));
                            reportWriter.AddScreenShot(TakeScreenshot(featureContext, _scenarioContext));


                            dr = dt.NewRow();
                            dr["Plan"] = featureContext.FeatureInfo.Title;
                            dr["Test Case"] = scenarioContext.ScenarioInfo.Title;
                            dr["Step"] = scenarioContext.StepContext.StepInfo.Text;
                            dr["Status"] = "Fail";
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        if (ExtentReportFlag)
                        {
                            reportWriter.LogTestCaseInfo(ReportStepBody(scenarioContext, true, message));
                        }
                        dr = dt.NewRow();
                        dr["Plan"] = featureContext.FeatureInfo.Title;
                        dr["Test Case"] = scenarioContext.ScenarioInfo.Title;
                        dr["Step"] = scenarioContext.StepContext.StepInfo.Text;
                        dr["Status"] = "Pass";
                        dt.Rows.Add(dr);
                    }
                }
                catch (Exception ex)
                {
                    featureResult = false;
                    if (ExtentReportFlag)
                    {
                        reportWriter.FailTestCase(ReportStepBody(scenarioContext, false, errormessage));
                        reportWriter.AddScreenShot(TakeScreenshot(featureContext, _scenarioContext));
                    }
                }
            }

        }

        [AfterFeature]
        public static void CloseApplication(FeatureContext featureContext)
        {
            //StepBase.Driver.Quit();
            Logger.LogInfo(string.Format("SUITE EXECUTION HAS BEEN COMPLETED."));
            if (ExtentReportFlag)
            {
                reportWriter.CloseTestPlan("Pass", string.Format("<b>TEST PLAN</b> {0} HAS BEEN COMPLETED", featureContext.FeatureInfo.Title));
                reportWriter.SaveTestCaseReport(featureContext.FeatureInfo.Title);
            }
        }

        [AfterScenario]
        public static void SetResults(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            Browser.Close();
            if (ExtentReportFlag)
            {
                string testCaseReportFolder = string.Format("{0}", featureContext.FeatureInfo.Title).Replace(" ", "_");
                try
                {
                    var Result = scenarioContext.ScenarioExecutionStatus;
                    if (Result.Equals(ScenarioExecutionStatus.TestError) || Result.Equals(ScenarioExecutionStatus.BindingError))
                    {
                        if (((NUnit.Framework.ResultStateException)scenarioContext.TestError).ResultState.Status.Equals(NUnit.Framework.Interfaces.TestStatus.Passed))
                        {
                            reportWriter.CloseTestCase("Pass", string.Format("TEST CASE <i>{0}</i>: HAS BEEN COMPLETED SUCCESSFULLY", scenarioContext.ScenarioInfo.Title));
                            reportWriter.LogInfo(string.Format("TEST CASE [<a href=\"./{1}/index.html\">{0}</a>] HAS BEEN PASSED.", scenarioContext.ScenarioInfo.Title.ToUpper(), testCaseReportFolder));

                        }
                        else
                        {
                            reportWriter.CloseTestCase("Fail", string.Format("TEST CASE <i>{0}</i>: HAS BEEN FAILED", scenarioContext.ScenarioInfo.Title));
                            reportWriter.LogError(string.Format("TEST CASE [<a href=\"./{1}/index.html\">{0}</a>] HAS BEEN FAILED.", scenarioContext.ScenarioInfo.Title.ToUpper(), testCaseReportFolder));

                        }

                    }
                    else
                    {
                        reportWriter.CloseTestCase("Pass", string.Format("TEST CASE <i>{0}</i>: HAS BEEN COMPLETED SUCCESSFULLY", scenarioContext.ScenarioInfo.Title));
                        reportWriter.LogInfo(string.Format("TEST CASE [<a href=\"./{1}/index.html\">{0}</a>] HAS BEEN PASSED.", scenarioContext.ScenarioInfo.Title.ToUpper(), testCaseReportFolder));

                    }

                    foreach (string tag in scenarioContext.ScenarioInfo.Tags)
                    {
                        if (Regex.IsMatch(tag, @"GMU-\d{3}"))
                        {
                            string testKey = tag.Split('_').Last();
                            int testStatus = (int)_scenarioContext.ScenarioExecutionStatus;

                            // ZephyrUtils.ChangeTestStatus(testKey, testStatus);
                        }
                    }
                }
                catch (Exception)
                {
                    reportWriter.CloseTestCase("Fail", string.Format("TEST CASE <i>{0}</i>: HAS BEEN FAILED", scenarioContext.ScenarioInfo.Title));
                    reportWriter.LogError(string.Format("TEST CASE [<a href=\"./{1}/index.html\">{0}</a>] HAS BEEN FAILED.", scenarioContext.ScenarioInfo.Title.ToUpper(), testCaseReportFolder));

                }
            }

        }

        [AfterTestRun]

        public static string ReportStepBody(ScenarioContext scenarioContext, bool status = true, string message = "")
        {

            string body = string.Empty;
            if (status)
                body += string.Format("<b style='color:#0F0;'>Pass</b><br />");
            else
                body += string.Format("<b style='color:#F00;'>Fail</b><br />");
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();

            body = string.Format("#{0} ", stepType + " " + scenarioContext.StepContext.StepInfo.Text);

            if (message != "")
            {
                body += string.Format("<br />See {0}", message);
            }
            return body;
        }
        public static string TakeScreenshot(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            string imagePath = GetScreenshotImageName(string.Format("{0}_{1}", featureContext.FeatureInfo.Title, scenarioContext.ScenarioInfo.Title));

            var artifactDirectory = Path.Combine(config.ScreenShotFolder, imagePath);
            if (!Directory.Exists(artifactDirectory))
                Directory.CreateDirectory(artifactDirectory);

            string pageSource = Browser.Get(StepBase.browserType).PageSource;

            Screenshot screen = ((ITakesScreenshot)Browser.Get(StepBase.browserType)).GetScreenshot();
            string screenshotFilePath = ReplaceInvalidChars(scenarioContext.StepContext.StepInfo.Text) + ".Png";
            string localpath = Path.Combine(artifactDirectory, screenshotFilePath);

            screen.SaveAsFile(localpath);
            return localpath;
        }

        public static string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }


        protected static string GetScreenshotImageName(string itemName)
        {
            return string.Format("{0}_{1}", itemName.Replace(" ", "_"), DateTime.Now.ToString("yyyyMMddHHmmss"));
        }
    }
}
