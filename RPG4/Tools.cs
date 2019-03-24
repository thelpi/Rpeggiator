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
        /// Computes the value of a string mathematic formula.
        /// </summary>
        /// <typeparam name="T">Type of formula result.</typeparam>
        /// <param name="formulaStringValue">Formula.</param>
        /// <param name="substitutions">Subtitutions to make in the formula.</param>
        /// <returns>Formula result.</returns>
        public static T ComputeFormulaResult<T>(string formulaStringValue, params Tuple<string, object>[] substitutions)
        {
            for (int i = 0; i < substitutions.Length; i++)
            {
                formulaStringValue = formulaStringValue.Replace(substitutions[i].Item1, substitutions[i].Item2.ToString());
            }

            return (T)Convert.ChangeType(new DataTable().Compute(formulaStringValue, string.Empty), typeof(T));
        }
    }
}
