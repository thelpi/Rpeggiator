using System;
using System.Data.SQLite;

namespace RpeggiatorLib
{
    /// <summary>
    /// Extension methods for <see cref="SqliteMapper"/>.
    /// </summary>
    internal static class SqliteMapperExtensions
    {
        /// <summary>
        /// Extracts a nullable value from a <see cref="SQLiteDataReader"/> at a specified column.
        /// </summary>
        /// <typeparam name="T">The nullable output type.</typeparam>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value.</returns>
        internal static T? GetNullValue<T>(this SQLiteDataReader reader, string columnName) where T : struct
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? (T?)null
                : (T)Convert.ChangeType(reader[columnName], typeof(T));
        }

        /// <summary>
        /// Extracts a non-nullable value from a <see cref="SQLiteDataReader"/> at a specified column.
        /// </summary>
        /// <typeparam name="T">The output type.</typeparam>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value.</returns>
        internal static T GetValue<T>(this SQLiteDataReader reader, string columnName) where T : struct
        {
            object nonTypedValue = reader[columnName];
            if (nonTypedValue == null || nonTypedValue == DBNull.Value)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(nonTypedValue, typeof(T));
        }

        /// <summary>
        /// Gets a value of type <see cref="double"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="double"/>.</returns>
        internal static double GetDouble(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetValue<double>(columnName);
        }

        /// <summary>
        /// Gets a value of type <see cref="int"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="int"/>.</returns>
        internal static int GetInt32(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetValue<int>(columnName);
        }

        /// <summary>
        /// Gets a value of type <see cref="string"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="string"/>.</returns>
        internal static string GetString(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetString(reader.GetOrdinal(columnName));
        }
    }
}
