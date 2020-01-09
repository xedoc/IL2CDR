using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace IL2CDR.Model
{
	public class AirField
	{
		public int Id { get; set; }
		public Vector3D Position { get; set; }
		public Country Country { get; set; }
	}
}