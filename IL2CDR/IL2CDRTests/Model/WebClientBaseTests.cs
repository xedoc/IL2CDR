using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL2CDR.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace IL2CDR.Model.Tests
{
    [TestClass()]
    public class WebClientBaseTests
    {
        [TestMethod()]
        public void GZipBytesTest()
        {
            var webClient = new WebClientBase();
            var compressed = webClient.GZipBytes("test");
            var decompressed = webClient.GUnzip(compressed);

            Assert.IsTrue("test" == decompressed);

        }
    }
}
