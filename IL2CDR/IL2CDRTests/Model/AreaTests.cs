using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;
using IL2CDR.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
namespace IL2CDR.Model.Tests
{
    [TestClass()]
    public class AreaTests
    {
        [TestMethod()]
        public void AreaTest()
        {

        }

        [TestMethod()]
        public void InBoundsTest()
        {
            var testSquare = new Vector3D[] {
                new Vector3D(0,0,0),
                new Vector3D(0,0,1),
                new Vector3D(1,0,1),
                new Vector3D(1,0,0),
                new Vector3D(0.5,0,0.5),
            };

            var area = new Area(testSquare);

            Assert.IsFalse(area.InBounds(new Vector3D(2, 0.0, 2)));
            Assert.IsTrue(area.InBounds(new Vector3D(0.1, 0.0, 0.9)));
            Assert.IsFalse(area.InBounds(new Vector3D(0.4, 0.0, 0.4)));
        }
    }
}
