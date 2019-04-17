using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

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
                    // Keep this code if initialization required.
                    /*using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = Properties.Resources.SpriteDb_sql;

                        cmd.ExecuteNonQuery();
                    }*/
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(Messages.InvalidSqliteConnectionExceptionMessage, ex.Message, ex.StackTrace));
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

            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    List<Sprites.PermanentStructure>  permanentStructures = GetPermanentStructures(id, cmd);
                    List<Sprites.Door> doors = GetDoors(id, cmd);
                    List<Sprites.Floor> floors = GetFloors(id, cmd);
                    List<Sprites.Enemy> enemies = GetEnemies(id, cmd);
                    List<Sprites.GateTrigger> gateTriggers = GetGateTriggers(id, cmd);
                    List<Sprites.Gate> gates = GetGates(id, cmd);
                    List<Sprites.Rift> rifts = GetRifts(id, cmd);
                    List<Sprites.Pit> pits = GetPits(id, cmd);
                    List<Sprites.Chest> chests = GetChests(id, cmd);
                    List<Sprites.PickableItem> pickableItems = GetPickableItems(id, cmd);

                    cmd.CommandText = string.Format(
                        "select x, y, width, height, floor_type, darkness_opacity, render_type, {0}, " +
                        "neighboring_screen_top, neighboring_screen_bottom, neighboring_screen_right, neighboring_screen_left " +
                        "from screen where id = @id", string.Join(", ", GenerateSqlColumnsRender())
                    );
                    cmd.Parameters.Add("@id", DbType.Int32);
                    cmd.Parameters["@id"].Value = id;

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Dictionary<Enums.Direction, int> neighboringScreens = new Dictionary<Enums.Direction, int>
                            {
                                { Enums.Direction.Bottom, reader.GetInt32(18) },
                                { Enums.Direction.Top, reader.GetInt32(17) },
                                { Enums.Direction.Right, reader.GetInt32(19) },
                                { Enums.Direction.Left, reader.GetInt32(20) }
                            };

                            s = new Sprites.Screen(id, reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2),
                                reader.GetDouble(3), (Enums.FloorType)reader.GetInt32(4), reader.GetDouble(5), reader.GetString(6),
                                GetRenderPropertiesForCurrentReaderRow(reader, 7), permanentStructures, doors, floors, enemies,
                                gateTriggers, gates, rifts, pits, chests, pickableItems, neighboringScreens);
                        }
                    }
                }
            }

            return s;
        }

        #region Get screen informations by sprite type.

        // Gets permanent structures of a screen.
        private List<Sprites.PermanentStructure> GetPermanentStructures(int id, SQLiteCommand cmd)
        {
            List<Sprites.PermanentStructure> sprites = new List<Sprites.PermanentStructure>();

            cmd.CommandText = GenerateSpriteTableSql("permanent_structure", null);
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.PermanentStructure(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5)));
                }
            }

            return sprites;
        }

        // Gets doors of a screen.
        private List<Sprites.Door> GetDoors(int id, SQLiteCommand cmd)
        {
            List<Sprites.Door> sprites = new List<Sprites.Door>();

            List<string> otherColumns = new List<string>
            {
                "key_id", "connected_screen_id", "id", "player_go_through_x", "player_go_through_y", "locked_render_type"
            };
            otherColumns.AddRange(GenerateSqlColumnsRender("locked_render_value"));
            cmd.CommandText = GenerateSpriteTableSql("door", otherColumns.ToArray());
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Door(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetNullValue<int>(15), reader.GetInt32(16), reader.GetInt32(17), reader.GetDouble(18),
                        reader.GetDouble(19), reader.GetString(20), GetRenderPropertiesForCurrentReaderRow(reader, 21),
                        reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5)));
                }
            }

            return sprites;
        }

        // Gets chests of a screen.
        private List<Sprites.Chest> GetChests(int id, SQLiteCommand cmd)
        {
            List<Sprites.Chest> sprites = new List<Sprites.Chest>();

            List<string> otherColumns = new List<string>
            {
                "item_type", "quantity", "key_id", "key_id_container", "open_render_type"
            };
            otherColumns.AddRange(GenerateSqlColumnsRender("open_render_value"));
            cmd.CommandText = GenerateSpriteTableSql("chest", otherColumns.ToArray());
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Chest(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        (Enums.ItemType?)reader.GetNullValue<int>(15), reader.GetInt32(16), reader.GetNullValue<int>(17),
                        reader.GetNullValue<int>(18), reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5),
                        reader.GetString(19), GetRenderPropertiesForCurrentReaderRow(reader, 20)));
                }
            }

            return sprites;
        }

        // Gets enemies of a screen.
        private List<Sprites.Enemy> GetEnemies(int id, SQLiteCommand cmd)
        {
            List<Sprites.Enemy> sprites = new List<Sprites.Enemy>();

            cmd.CommandText = GenerateSpriteTableSqlWithoutRender("enemy", "maximal_life_points", "hit_life_point_cost",
                "speed", "recovery_time", "render_filename", "render_recovery_filename", "default_direction",
                "loot_item_type", "loot_quantity", "id");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Enemy(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetDouble(4), reader.GetDouble(5), reader.GetDouble(6), reader.GetDouble(7),
                        reader.GetString(8), reader.GetString(9), (Enums.Direction)reader.GetInt32(10),
                        (Enums.ItemType?)reader.GetNullValue<int>(11), reader.GetInt32(12)));

                    sprites.Last().SetPath(GetEnemyPathSteps(reader.GetInt32(13)));
                }
            }

            return sprites;
        }

        // gets path steps for an enemy.
        private List<Point> GetEnemyPathSteps(int enemyId)
        {
            List<Point> pointOfSteps = new List<Point>();

            using (SQLiteConnection connection2 = new SQLiteConnection(CONN_STRING))
            {
                connection2.Open();
                using (SQLiteCommand cmd2 = connection2.CreateCommand())
                {
                    cmd2.CommandText = string.Format(
                        "select x, y from enemy_step where enemy_id = {0} order by step_no asc", enemyId);

                    using (SQLiteDataReader reader2 = cmd2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            pointOfSteps.Add(new Point(reader2.GetDouble(0), reader2.GetDouble(1)));
                        }
                    }
                }
            }

            return pointOfSteps;
        }

        // Gets pits of a screen.
        private List<Sprites.Pit> GetPits(int id, SQLiteCommand cmd)
        {
            List<Sprites.Pit> sprites = new List<Sprites.Pit>();

            cmd.CommandText = GenerateSpriteTableSql("pit", "screen_index_entrance");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Pit(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetNullValue<int>(15), reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5)));
                }
            }

            return sprites;
        }

        // Gets floors of a screen.
        private List<Sprites.Floor> GetFloors(int id, SQLiteCommand cmd)
        {
            List<Sprites.Floor> sprites = new List<Sprites.Floor>();

            cmd.CommandText = GenerateSpriteTableSql("floor", "floor_type");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Floor(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        (Enums.FloorType)reader.GetInt32(15), reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5)));
                }
            }

            return sprites;
        }

        // Gets pickable items of a screen.
        private List<Sprites.PickableItem> GetPickableItems(int id, SQLiteCommand cmd)
        {
            List<Sprites.PickableItem> sprites = new List<Sprites.PickableItem>();

            cmd.CommandText = GenerateSpriteTableSqlWithoutRender("pickable_items",
                "item_type", "quantity", "time_before_disapear");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.PickableItem(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        (Enums.ItemType?)reader.GetNullValue<int>(4), reader.GetInt32(5), reader.GetNullValue<double>(6)));
                }
            }

            return sprites;
        }

        // Gets gates of a screen.
        private List<Sprites.Gate> GetGates(int id, SQLiteCommand cmd)
        {
            List<Sprites.Gate> sprites = new List<Sprites.Gate>();

            cmd.CommandText = GenerateSpriteTableSql("gate", "activated");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Gate(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetInt32(15) > 0, reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5)));
                }
            }

            return sprites;
        }

        // Gets rifts of a screen.
        private List<Sprites.Rift> GetRifts(int id, SQLiteCommand cmd)
        {
            List<Sprites.Rift> sprites = new List<Sprites.Rift>();

            cmd.CommandText = GenerateSpriteTableSql("rift", "lifepoints");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Rift(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetDouble(15), reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5)));
                }
            }

            return sprites;
        }

        // Gets gate triggers of a screen.
        private List<Sprites.GateTrigger> GetGateTriggers(int id, SQLiteCommand cmd)
        {
            List<Sprites.GateTrigger> sprites = new List<Sprites.GateTrigger>();

            List<string> otherColums = new List<string>
            {
                "action_duration", "gate_id", "appear_on_activation", "on_render_type"
            };
            otherColums.AddRange(GenerateSqlColumnsRender("on_render_value"));
            cmd.CommandText = GenerateSpriteTableSql("gate_trigger", otherColums.ToArray());
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.GateTrigger(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetDouble(15), reader.GetInt32(16), reader.GetInt32(17) > 0,
                        reader.GetString(4), GetRenderPropertiesForCurrentReaderRow(reader, 5),
                        reader.GetString(18), GetRenderPropertiesForCurrentReaderRow(reader, 19)));
                }
            }

            return sprites;
        }

        #endregion

        #region Static tool methods

        // Generates SQL query to get informations about a certain type of sprite relatives to a screen.
        private static string GenerateSpriteTableSql(string table, params string[] additionalColumns)
        {
            return string.Format(
                "select x, y, width, height, render_type {0} {1} from {2} where screen_id = @screen_id",
                string.Concat(", ", string.Join(", ", GenerateSqlColumnsRender())),
                (additionalColumns?.Length > 0 ? ", " + string.Join(", ", additionalColumns) : string.Empty),
                table);
        }

        // Generates SQL query to get informations about a certain type of sprite relatives to a screen; no columns relatives to render.
        private static string GenerateSpriteTableSqlWithoutRender(string table, params string[] additionalColumns)
        {
            return string.Format(
                "select x, y, width, height {0} from {1} where screen_id = @screen_id",
                (additionalColumns?.Length > 0 ? ", " + string.Join(", ", additionalColumns) : string.Empty),
                table);
        }

        // Generates the SQL which contains every columns related to "render_value" (or similar pattern).
        private static List<string> GenerateSqlColumnsRender(string pattern = "render_value")
        {
            List<string> cols = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                cols.Add(string.Format("{0}_{1}", pattern, i));
            }
            return cols;
        }

        // Builds an array of values relatives to the render
        private static object[] GetRenderPropertiesForCurrentReaderRow(SQLiteDataReader reader, int startAt)
        {
            object[] renderProperties = new object[10];
            for (int i = 0; i < 10; i++)
            {
                renderProperties[i] = reader[startAt + i];
            }
            return renderProperties;
        }

        // Generates SQL query to insert datas in a table. Parameters have the same name as columns with "@" prefix.
        private static string GenerateSqlInsert(string tableName, params string[] columns)
        {
            return string.Format("insert into {0} ({1}) values ({2})",
                tableName,
                string.Join(", ", columns),
                string.Join(", ", columns.Select(c => string.Concat("@", c))));
        }

        // Creates, prepares and executes a insert SQL statement for multiple insertions.
        private static void ExecutePreparedInsert(string tableName, string[] columns, DbType[] types, IEnumerable<object[]> rowsValues)
        {
            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = GenerateSqlInsert(tableName, columns);
                    for (int i = 0; i < columns.Length; i++)
                    {
                        cmd.Parameters.Add(columns[i].ToParam(), types[i]);
                    }
                    cmd.Prepare();

                    foreach (object[] rowValue in rowsValues)
                    {
                        for (int i = 0; i < columns.Length; i++)
                        {
                            cmd.Parameters[columns[i].ToParam()].Value = rowValue[i];
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        #region Insert methods

        /// <summary>
        /// Creates or recreates every steps on an <see cref="Sprites.Enemy"/> path.
        /// </summary>
        /// <param name="enemyId"><see cref="Sprites.Enemy"/> identifier.</param>
        /// <param name="points">List of coordinates; the key indicates the order.</param>
        /// <exception cref="ArgumentException"><paramref name="enemyId"/> is lower or equals to zero.</exception>
        public void GetEnemyPathSteps(int enemyId, Dictionary<int, System.Windows.Point> points)
        {
            if (enemyId <= 0)
            {
                throw new ArgumentException(nameof(enemyId));
            }

            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    // Removes previous steps
                    cmd.CommandText = "delete from enemy_step where enemy_id = @enemy_id";
                    cmd.Parameters.Add("@enemy_id", DbType.Int32);
                    cmd.Parameters["@enemy_id"].Value = enemyId;
                    cmd.ExecuteNonQuery();
                }

                if (points != null)
                {
                    points = points.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    ExecutePreparedInsert("enemy_step",
                        new[] { "enemy_id", "step_no", "x", "y" },
                        new[] { DbType.Int32, DbType.Int32, DbType.Double, DbType.Double },
                        points.Select(kvp => new object[] { enemyId, kvp.Key, kvp.Value.X, kvp.Value.Y })
                    );
                }
            }
        }

        #endregion
    }

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
        /// <param name="index">Column index in the data reader.</param>
        /// <returns>Value.</returns>
        internal static T? GetNullValue<T>(this SQLiteDataReader reader, int index) where T : struct
        {
            return reader.IsDBNull(index) ? (T?)null : (T)Convert.ChangeType(reader[index], typeof(T));
        }

        /// <summary>
        /// Transforms a <see cref="string"/> which represents a SQL column, to its associated parameter name.
        /// </summary>
        internal static string ToParam(this string column)
        {
            return string.Concat("@", column);
        }
    }
}
