using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IL2CDR.Model;
using IL2CDR.Scripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace IL2CDR.Scripts.Tests
{
    [TestClass()]
    public class AntiFreezeTests
    {
        [TestMethod()]
        public void AntiFreezeTest()
        {
        }

        [TestMethod()]
        public void OnServerStopTest()
        {

        }

        [TestMethod()]
        public void OnServerStartTest()
        {
            var antiFreeze = new AntiFreeze();
            antiFreeze.Config = antiFreeze.DefaultConfig;
            antiFreeze.Config.ConfigFields.Set("RestartTimeout", 1);
            var id = 6192;
            var process = Process.GetProcessById(id);
            var cmdLine = process.CommandLine();

            antiFreeze.OnServerStart(
                new Server("Test server", true, true) 
                { 
                    Process = new ProcessItem( id,"Test server", process.ProcessName, cmdLine) 
                });

            Thread.Sleep(60000);
        }

        [TestMethod()]
        public void OnHistoryTest()
        {

        }
    }
}
