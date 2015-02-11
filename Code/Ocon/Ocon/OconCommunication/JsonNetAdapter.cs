using Newtonsoft.Json;

namespace Ocon.OconCommunication
{
    class JsonNetAdapter : IOconSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonNetAdapter(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.Objects };
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
