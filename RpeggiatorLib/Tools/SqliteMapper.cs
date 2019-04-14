using System.Collections.Generic;
using System.Data.SQLite;

namespace RpeggiatorLib
{
    /// <summary>
    /// Mapper for Sqlite database.
    /// </summary>
    public class SqliteMapper
    {
        /// <summary>
        /// Database name.
        /// </summary>
        public const string DB_NAME = "SpriteDb.sqlite";
        // Connection string.
        private static readonly string CONN_STRING = string.Format("Data Source={0};Version=3;", DB_NAME);

        private static SqliteMapper _default = null;

        /// <summary>
        /// Default instance (singleton).
        /// </summary>
        public static SqliteMapper Defaut
        {
            get
            {
                if (_default == null)
                {
                    _default = new SqliteMapper();
                }

                return _default;
            }
        }

        // Private constructor.
        private SqliteMapper()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
                {
                    connection.Open();
                }
            }
            catch (System.Exception ex)
            {
                throw new System.InvalidOperationException(string.Format(Messages.InvalidSqliteConnectionExceptionMessage, ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Gets a <see cref="Sprites.Screen"/> by its identifier.
        /// </summary>
        /// <param name="id"><see cref="Sprites.Screen"/> identifier.</param>
        /// <returns><see cref="Sprites.Screen"/></returns>
        internal Sprites.Screen GetScreenById(int id)
        {
            Sprites.Screen s = null;

            List<Sprites.PermanentStructure> permanentStructures = new List<Sprites.PermanentStructure>();
            List<Sprites.Door> doors = new List<Sprites.Door>();
            List<Sprites.Floor> floors = new List<Sprites.Floor>();
            List<Sprites.Enemy> enemies = new List<Sprites.Enemy>();
            List<Sprites.GateTrigger> gateTriggers = new List<Sprites.GateTrigger>();
            List<Sprites.Gate> gates = new List<Sprites.Gate>();
            List<Sprites.Rift> rifts = new List<Sprites.Rift>();
            List<Sprites.Pit> pits = new List<Sprites.Pit>();
            List<Sprites.Chest> chests = new List<Sprites.Chest>();
            List<Sprites.PickableItem> pickableItems = new List<Sprites.PickableItem>();
            List<Sprites.ActionnedItem> actionnedItems = new List<Sprites.ActionnedItem>();

            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = string.Format(
                        "select x, y, width, height, floor_type, darkness_opacity, render_type, " +
                        "render_value_0, render_value_1, render_value_2, render_value_3, render_value_4, " +
                        "render_value_5, render_value_6, render_value_7, render_value_8, render_value_9, " +
                        "neighboring_screen_top, neighboring_screen_bottom, neighboring_screen_right, neighboring_screen_left " +
                        "from screen " +
                        "where id = {0}", id
                    );

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Dictionary<Enums.Direction, int> neighboringScreens = new Dictionary<Enums.Direction, int>();
                            neighboringScreens.Add(Enums.Direction.Bottom, reader.GetInt32(18));
                            neighboringScreens.Add(Enums.Direction.Top, reader.GetInt32(17));
                            neighboringScreens.Add(Enums.Direction.Right, reader.GetInt32(19));
                            neighboringScreens.Add(Enums.Direction.Left, reader.GetInt32(20));

                            s = new Sprites.Screen(id, reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2),
                                reader.GetDouble(3), (Enums.FloorType)reader.GetInt32(4), reader.GetDouble(5), reader.GetString(6),
                                GetRenderPropertiesForCurrentReaderRow(reader, 7), permanentStructures, doors, floors, enemies,
                                gateTriggers, gates, rifts, pits, chests, pickableItems, actionnedItems, neighboringScreens);
                        }
                    }
                }
            }

            return s;
        }

        // Builds an array of values relatives to the render
        private object[] GetRenderPropertiesForCurrentReaderRow(SQLiteDataReader reader, int startAt)
        {
            object[] renderProperties = new object[10];
            for (int i = 0; i < 10; i++)
            {
                renderProperties[i] = reader[startAt + i];
            }
            return renderProperties;
        }
    }
}
