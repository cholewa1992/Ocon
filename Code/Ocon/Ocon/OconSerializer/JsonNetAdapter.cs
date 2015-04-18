using System.IO;
using System.Reflection;
using Aq.ExpressionJsonSerializer;
using Newtonsoft.Json;

namespace Ocon.OconSerializer
{
    public class JsonNetAdapter : IOconSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonNetAdapter(JsonSerializerSettings settings = null)
        {
            var defaultSettings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};
            defaultSettings.Converters.Add(new ExpressionJsonConverter(Assembly.GetAssembly(typeof(IOconSituation))));
            _settings = settings ?? defaultSettings;
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public T Deserialize<T>(Stream stream)
        {
            var sr = new StreamReader(stream);
            var jsonReader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();

            return serializer.Deserialize<T>(jsonReader);
        }
    }
}
