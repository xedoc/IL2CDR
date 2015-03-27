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

            Assert.IsTrue(true);
        }
    }
}
