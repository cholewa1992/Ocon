using System.IO;

namespace Ocon.OconSerializer
{
    public interface IOconSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}
