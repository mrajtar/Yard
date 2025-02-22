using Newtonsoft.Json;
using System.Text.Json;

namespace Yard.config
{
    internal class JSONReader
    {
        public async Task<JSONStructure> ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                return data;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }

    }
}
