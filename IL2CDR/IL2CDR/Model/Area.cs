﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace IL2CDR.Model
{
	public class Area
	{
		private double minX, minY, maxX, maxY;

		public Area(Vector3D[] boundaries)
		{
			this.SetBoundaries(boundaries);
		}

		public Area(int id, Country country, bool isEnabled)
		{
			this.Id = id;
			this.Country = country;
			this.IsEnabled = isEnabled;
			this.Boundaries = new Vector3D[] { };
		}

		public int Coalition { get; set; }
		public Country Country { get; set; }
		public int Id { get; set; }
		public Vector3D[] Boundaries { get; set; }
		public bool IsEnabled { get; set; }

		public void SetBoundaries(Vector3D[] boundaries)
		{
			this.minX = boundaries.Min(v => v.X);
			this.minY = boundaries.Min(v => v.Z);
			this.maxX = boundaries.Max(v => v.X);
			this.maxY = boundaries.Max(v => v.Z);
			this.Boundaries = new Vector3D[boundaries.Length];
			boundaries.CopyTo(this.Boundaries, 0);
		}

		public bool InBounds(Vector3D point)
		{
			if (this.Boundaries.Length <= 0) {
				return false;
			}

			if (point.X < this.minX ||
				point.X > this.maxX ||
				point.Z < this.minY ||
				point.Z > this.maxY) {
				return false;
			}

			var length = this.Boundaries.Length;
			var result = false;
			for (int i = 0, j = length - 1; i < length; j = i++) {
				if (this.Boundaries[i].Z > point.Z != this.Boundaries[j].Z > point.Z &&
					point.X < (this.Boundaries[j].X - this.Boundaries[i].X) * (point.Z - this.Boundaries[i].Z) /
					(this.Boundaries[j].Z - this.Boundaries[i].Z) + this.Boundaries[i].X) {
					result = !result;
				}
			}

			return result;
		}
	}
}