using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
	public class Country
	{
		public Country(int id)
		{
			this.Id = id;
			this.Name = this.GetCountryName(id);
		}

		public int Id { get; set; }
		public string Name { get; set; }

		private string GetCountryName(int id)
		{
			switch (id) {
				case 0:
					return "Neutral";
				case 101:
					return "Russia";
				case 201:
					return "Germany";
				case 202:
					return "Italy";
			}

			return "Unknown";
		}
	}
}