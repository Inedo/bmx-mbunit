using System;
using System.IO;
using System.Xml;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Extensibility.Actions.Testing;
using Inedo.BuildMaster.Web;

namespace Inedo.BuildMasterExtensions.MbUnit
{
    /// <summary>
    /// Represents an action that can run an MbUnit test.
    /// </summary>
    [ActionProperties(
        "Execute MbUnit2 Tests",
        "Runs MbUnit unit tests on a specified project, assembly, or MbUnit file.",
        "Testing")]
    [CustomEditor(typeof(MbUnitActionEditor))]
    public sealed class MbUnitAppAction : UnitTestActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MbUnitAppAction"/> class.
        /// </summary>
        public MbUnitAppAction()
        {
        }

        /// <summary>
        /// Gets or sets the test runner exe path.
        /// </summary>
        [Persistent]
        public string ExePath { get; set; }
        /// <summary>
        /// Gets or sets the file mbunit will test against (could be dll, proj, or config file based on test runner)
        /// </summary>
        [Persistent]
        public string TestFile { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Run MbUnit Unit Tests on {0}", this.TestFile);
        }

        protected override void RunTests()
        {
            var fileName = Guid.NewGuid().ToString();

            var start = DateTime.Now;

            this.MBUnit("{0} /rt:xml /rf:\"{1}\" /rnf:{2}",
                Path.Combine(this.RemoteConfiguration.SourceDirectory, this.TestFile),
                this.RemoteConfiguration.TempDirectory,
                fileName
            );

            bool result = true;

            var doc = new XmlDocument();
            doc.Load(Path.Combine(this.RemoteConfiguration.TempDirectory, fileName + ".xml"));

            var testStart = DateTime.Now;

            var nodeList = doc.SelectNodes("//run");

            foreach (XmlNode node in nodeList)
            {
                var testName = node.Attributes["name"].Value;

                bool nodeResult = node.Attributes["result"].Value == "success";
                if (!nodeResult)
                    result = false;

                var testLength = double.Parse(node.Attributes["duration"].Value);

                this.LogInformation("MbUnit Test: {0}, Result: {1}, Test Length: {2} s",
                    testName,
                    nodeResult,
                    testLength);

                this.RecordResult(
                    testName,
                    nodeResult,
                    node.OuterXml,
                    testStart,
                    testStart.AddSeconds(testLength)
                );

                testStart = testStart.AddSeconds(testLength);
            }

            if (result == false)
                this.LogError("Unit test failed");
        }

        private void MBUnit(string argsFormat, params string[] args)
        {
            var workingDir = this.RemoteConfiguration.TempDirectory;
            if (!File.Exists(this.ExePath))
                throw new InvalidOperationException(string.Format("Could not find MbUnit ({0}).", this.ExePath));

            this.ExecuteCommandLine(this.ExePath, string.Format(argsFormat, args), this.RemoteConfiguration.SourceDirectory);
        }
    }
}
