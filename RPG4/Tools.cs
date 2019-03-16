using Newtonsoft.Json;
using RPG4.Properties;

namespace RPG4
{
    public static class Tools
    {
        public static dynamic GetScreenDatasFromIndex(int screenIndex)
        {
            return JsonConvert.DeserializeObject(Resources.ResourceManager.GetString(string.Concat("Screen", screenIndex)));
        }
    }
}
