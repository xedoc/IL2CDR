using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
	public class AirFieldCollection : Dictionary<int, AirField>
	{
		private readonly object lockList = new object();

		public new AirField this[int id]
		{
			get
			{
				AirField result = null;
				lock (this.lockList) {
					this.TryGetValue(id, out result);
				}

				return result;
			}
			set
			{
				if (value == null || value.Id <= 0) {
					return;
				}

				lock (this.lockList) {
					var existing = this[value.Id];
					if (existing == null) {
						this.Add(value.Id, value);
					} else {
						existing = value;
					}
				}
			}
		}
	}
}