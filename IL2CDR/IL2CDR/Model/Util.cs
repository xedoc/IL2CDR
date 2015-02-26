using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
