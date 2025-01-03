using Newtonsoft.Json;
using System.Text.Json;

namespace Yard.config
{
    internal class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }
        
        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                this.token = data.token;
                this.prefix = data.prefix;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }

    }
}
