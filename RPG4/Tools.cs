using Newtonsoft.Json;
using RPG4.Properties;

namespace RPG4
{
    /// <summary>
    /// Tool methods. 
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Gets the screen datas by its index.
        /// </summary>
        /// <param name="screenIndex">Screen index.</param>
        /// <returns>Dynamic screen datas.</returns>
        public static dynamic GetScreenDatasFromIndex(int screenIndex)
        {
            return JsonConvert.DeserializeObject(Resources.ResourceManager.GetString(string.Concat("Screen", screenIndex)));
        }
    }
}
