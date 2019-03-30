using Newtonsoft.Json;
using RPG4.Properties;
using System;
using System.Data;

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

        /// <summary>
        /// Computes the distance made in diagonal relatively to the distance made straightforward.
        /// </summary>
        /// <param name="frameDistance">Distance made straightforward.</param>
        /// <returns>Diagonal distance.</returns>
        public static double FrameDiagonalDistance(double frameDistance)
        {
            return Math.Sqrt((frameDistance * frameDistance) / 2);
        }
    }
}
