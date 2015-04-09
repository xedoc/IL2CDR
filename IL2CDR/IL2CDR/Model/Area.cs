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
        private Vector3D[] vectors;
        private double minX, minY, maxX, maxY;
        public Area(Vector3D[] vectors )
        {
            minX = vectors.Min(v => v.X);
            minY = vectors.Min(v => v.Z);
            maxX = vectors.Max(v => v.X);
            maxY = vectors.Max(v => v.Z);
            this.vectors = new Vector3D[vectors.Length];
            vectors.CopyTo(this.vectors, 0 );
        }

        public bool InBounds( Vector3D point )
        {
            if( point.X < minX || 
                point.X > maxX ||
                point.Z < minY ||
                point.Z > maxY )
                return false;
            
            int length = vectors.Length;
            bool result = true;
            for (int i = 0, j = length - 1; i < length; j = i++)
            {
                if (((vectors[i].Y > point.Z) != (vectors[j].Y > point.Z)) &&
                 (point.X < (vectors[j].X - vectors[i].X) * (point.Z - vectors[i].Z) / (vectors[j].Z - vectors[i].Z) + vectors[i].X))
                    result = !result;
            }
            return result;

        }
    }
}
