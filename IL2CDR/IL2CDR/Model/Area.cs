using System;
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

        public Area(Vector3D[] boundaries )
        {
            SetBoundaries(boundaries);
        }
        public Area( int id, Country country, bool isEnabled )
        {
            this.Id = id;
            this.Country = country;
            this.IsEnabled = isEnabled;
            this.Boundaries = new Vector3D[] { };
        }

        public int Coalition { get;set; }
        public Country Country { get; set; }
        public int Id { get; set; }
        public Vector3D[] Boundaries { get; set; }
        public bool IsEnabled { get; set; }

        public void SetBoundaries(Vector3D[] boundaries)
        {
            minX = boundaries.Min(v => v.X);
            minY = boundaries.Min(v => v.Z);
            maxX = boundaries.Max(v => v.X);
            maxY = boundaries.Max(v => v.Z);
            this.Boundaries = new Vector3D[boundaries.Length];
            boundaries.CopyTo(this.Boundaries, 0);
        }

        public bool InBounds( Vector3D point )
        {
            if (Boundaries.Length <= 0)
                return false;

            if( point.X < minX || 
                point.X > maxX ||
                point.Z < minY ||
                point.Z > maxY )
                return false;

            int length = Boundaries.Length;
            bool result = false;
            for (int i = 0, j = length - 1; i < length; j = i++)
            {
                if (((Boundaries[i].Z > point.Z) != (Boundaries[j].Z > point.Z)) &&
                 (point.X < (Boundaries[j].X - Boundaries[i].X) * (point.Z - Boundaries[i].Z) / 
                 (Boundaries[j].Z - Boundaries[i].Z) + Boundaries[i].X))
                    result = !result;
            }
            return result;
        }

    }
}
