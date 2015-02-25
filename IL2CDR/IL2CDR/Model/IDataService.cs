using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IL2CDR.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
    }
}
