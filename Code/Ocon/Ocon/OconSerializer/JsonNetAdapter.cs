using System.Linq.Expressions;
using System.Reflection;
using Aq.ExpressionJsonSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ocon.OconSerializer
{
    public class JsonNetAdapter : IOconSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonNetAdapter(JsonSerializerSettings settings = null)
        {
            var defaultSettings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};
            defaultSettings.Converters.Add(new ExpressionJsonConverter(Assembly.GetAssembly(typeof(Situation<bool>))));
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
    }
}
