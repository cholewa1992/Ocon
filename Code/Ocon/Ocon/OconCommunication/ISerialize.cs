using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocon.OconCommunication
{
    interface ISerialize
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string s);
        object Deserialize(string s);
    }
}
