using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public interface IStopStart
    {
        void Start();
        void Stop();
        void Restart();
    }
}
