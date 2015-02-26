using System;
using System.Collections.Generic;
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
                return e;
            }

            return null;
        }
    }
}
