using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace IL2CDR.Model
{
    public class Util
    {
        public static Exception Try(Action action)
        {
            if (action == null)
                return null;

            try
            {
                action();
            }
            catch( Exception e)
            {
                Log.WriteError("Exception: {0}\n{1}", e.Message, e.StackTrace);
                return e;
            }

            return null;
        }
        public static Vector3DCollection BoundaryPointsToVectorCollection(string bp)
        {
            Vector3DCollection vectors = new Vector3DCollection();
            var matches = Regex.Matches(bp, @"\([\d|\.|,]+\)");
            foreach( Match match in matches)
            {
                var vector = POSToVector3D(match.Value);
                vectors.Add(vector);
            }

            return vectors;
        }
        public static int[] SequenceToIntArray( string seq )
        {
            var result = new int[]{};
            if (String.IsNullOrWhiteSpace(seq))
                return result;

            var values = Re.GetSubString(seq, @"([\d|\.|,]+)");
            if (String.IsNullOrWhiteSpace(values))
                return result;

            return values.Split(',')
                .Select(x => { int i = 0; int.TryParse(x, out i); return i; })
                .ToArray();


        }
        public static Vector3D POSToVector3D( string pos )
        {
            Vector3D result = new Vector3D();
            double x = 0.0, y = 0.0, z = 0.0;

            if (String.IsNullOrWhiteSpace(pos))
                return result;

            var values = Re.GetSubString(pos, @"([\d|\.|,]+)");

            if (String.IsNullOrWhiteSpace(values))
                return result;
            
            var xyz = values.Split(',');
            if (pos.Length == 3)
            {
                double.TryParse(xyz[0], out x);
                double.TryParse(xyz[1], out y);
                double.TryParse(xyz[2], out z);                
            }
            return new Vector3D(x, y, z);
        }

        public static DateTime ParseDate( string text )
        {
            if( String.IsNullOrEmpty( text ))
                return default(DateTime);

            Func<string,string,string,string,string,string,DateTime>convertFunc = (y,m,d,h,min,sec) => 
            { 
                return new DateTime(int.Parse(y), int.Parse(m), int.Parse(d), int.Parse(h), int.Parse(min), int.Parse(sec));
            };

            //2015-02-25_11-43-53
            var match = Regex.Match(text, @"(\d{4})-(\d{2})-(\d{2})_(\d{2})-(\d{2})-(\d{2})");
            if( match.Success && match.Groups.Count == 7)
            {
                   return convertFunc(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, 
                       match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);
            }
            return default(DateTime);
        }

        private static readonly DateTimeOffset GregorianCalendarStart = new DateTimeOffset(1582, 10, 15, 0, 0, 0, TimeSpan.Zero);

        public static string GetNewestFilePath( string folder, string mask )
        {
            if( Directory.Exists( folder ))
            {
                dynamic newestFile = null;
                Try(() => {
                    newestFile = Directory.GetFiles(folder, mask)
                        .Select(path => new { Path = path, Time = File.GetLastWriteTime(path) })
                        .OrderBy(file => file.Time).LastOrDefault();
                });
                if( newestFile != null )
                    return newestFile.Path;
            }

            return null;
        }

        public static string[] GetFilesSortedByTime( string folder, string mask, bool asc)
        {
            string[] result = new string[]{};

            Try(()=>{
                result = Directory.GetFiles( folder, mask )
                    .Select(path => new { Path = path, Time = File.GetLastWriteTime(path) })
                    .OrderBy(file => file.Time).Select(file => file.Path).ToArray();

                if (!asc)
                    result = result.Reverse().ToArray();
            });
            return result;
        }

    }
}
