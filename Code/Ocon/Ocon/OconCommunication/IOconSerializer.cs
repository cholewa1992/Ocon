using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocon.OconCommunication
{
    public interface IOconSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}
