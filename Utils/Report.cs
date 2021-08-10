using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace MMC.Utils
{
    public class ReportWriter
    {
        private ExtentTest _extentPlan;
        private ExtentTest _extentTestCase;
        private ExtentTest _extentTestNode;
        protected ExtentReports ReportObject { get; set; }

        protected string FilePath { get; set; }

        public ReportWriter(string filePath)
        {
            FilePath = filePath + "\\";
            ReportObject = new ExtentReports();
        }

        protected ExtentReports TestCaseWriterObject
        {
            get;
            set;
        }

        public void StartTestPlan(string testPlanName, string description)
        {
            _extentPlan = ReportObject.CreateTest(testPlanName, description);
            TestCaseWriterObject = new ExtentReports();
        }

        public void StartTestCase(string testPlanName, string testCaseName, string description)
        {
            _extentTestCase = TestCaseWriterObject.CreateTest(testCaseName, description);
        }

        public void LogTestCaseInfo(string message)
        {
            _extentTestCase.Info(message);
        }

        public void LogTestCaseError(string message)
        {
            _extentTestCase.Error(message);

        }
        public void FailTestCase(string message)
        {
            _extentTestCase.Fail(message);
        }


        public void CloseTestCase(string status, string description)
        {
            _extentTestCase.Log(status == "Pass" ? Status.Pass : Status.Fail, description);
        }

        public void PassTestCase(string description)
        {
            _extentTestCase.Pass(description);
        }

        public void SaveTestCaseReport(string testPlanName)
        {
            string testCaseReportFolder = FilePath + string.Format("{0}", testPlanName).Replace(" ", "_") + "\\";
            System.IO.Directory.CreateDirectory(testCaseReportFolder);

            ExtentHtmlReporter extentHtmlReporter = new ExtentHtmlReporter(testCaseReportFolder);
            TestCaseWriterObject.AttachReporter(extentHtmlReporter);
            TestCaseWriterObject.Flush();
        }

        public void LogInfo(string message)
        {
            _extentPlan.Info(message);
        }

        public void LogError(string message)
        {
            _extentPlan.Error(message);
        }

        public void CloseTestPlan(string status, string description)
        {
            _extentPlan.Log(status == "Pass" ? Status.Pass : Status.Fail, description);
        }

        public void SaveReport()
        {
            ExtentHtmlReporter extentHtmlReporter = new ExtentHtmlReporter(FilePath);
            ReportObject.AttachReporter(extentHtmlReporter);
            ReportObject.Flush();
        }

        public void AddScreenShot(string screenshotLocation)
        {
            _extentTestCase.Log(Status.Fail, "Snapshot below: " + _extentTestCase.AddScreenCaptureFromPath(screenshotLocation));
        }


        public void CreateNodeOfTestCase(string nodeName, string nodeDescription = "")
        {
            _extentTestNode = _extentTestCase.CreateNode(nodeName, nodeDescription);
        }
        public void AddScreenShotToTestNode(string screenshotLocation)
        {
            _extentTestNode.Log(Status.Fail, "Snapshot below: " + _extentTestNode.AddScreenCaptureFromPath(screenshotLocation));
        }
        public void PassTestNode(string description = "")
        {
            _extentTestNode.Pass(description);
        }
        public void FailTestNode(string description)
        {
            _extentTestNode.Fail(description);
        }
    }

}
