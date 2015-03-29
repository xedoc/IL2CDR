using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Scripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace IL2CDR.Scripts.Tests
{
    [TestClass()]
    public class LogParserTests
    {
        [TestMethod()]
        public void OpenMissionReportTest()
        {
            var parser = new LogParser();
            parser.OpenMissionReport();
            var globalstat = new GlobalStatistics();
            globalstat.Config = globalstat.DefaultConfig;

            globalstat.Config.ConfigFields.Set("token", "0695da663534558c209f052ac2af4112");
            foreach( var item in parser.History)
            {
                globalstat.AddToQueue(item);
            }

            Assert.IsTrue(true);
        }
    }
}
