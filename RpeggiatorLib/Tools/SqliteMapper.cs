using System;
using System.Collections.Generic;
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
                        "select x, y, width, height, floor_type, darkness_opacity, " +
                        "render_type, " + string.Join(", ", GenerateSqlColumnsRender()) + ", " +
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

            cmd.CommandText = GenerateSpriteTableSql(id, "permanent_structure", null);

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
            cmd.CommandText = GenerateSpriteTableSql(id, "door", otherColumns.ToArray());

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
            cmd.CommandText = GenerateSpriteTableSql(id, "chest", otherColumns.ToArray());

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

            cmd.CommandText = GenerateSpriteTableSqlWithoutRender(id, "enemy", "maximal_life_points", "hit_life_point_cost",
                "speed", "recovery_time", "render_filename", "render_recovery_filename", "default_direction",
                "loot_item_type", "loot_quantity", "id");

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int enemyId = reader.GetInt32(13);

                    sprites.Add(new Sprites.Enemy(
                        reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3),
                        reader.GetDouble(4), reader.GetDouble(5), reader.GetDouble(6), reader.GetDouble(7),
                        reader.GetString(8), reader.GetString(9), (Enums.Direction)reader.GetInt32(10),
                        (Enums.ItemType?)reader.GetNullValue<int>(11), reader.GetInt32(12)));

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
                                    pointOfSteps.Add(new Point(reader2.GetDouble(0), reader.GetDouble(1)));
                                }
                            }
                        }
                    }

                    sprites.Last().SetPath(pointOfSteps);
                }
            }

            return sprites;
        }

        // Gets pits of a screen.
        private List<Sprites.Pit> GetPits(int id, SQLiteCommand cmd)
        {
            List<Sprites.Pit> sprites = new List<Sprites.Pit>();

            cmd.CommandText = GenerateSpriteTableSql(id, "pit", "screen_index_entrance");

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

            cmd.CommandText = GenerateSpriteTableSql(id, "floor", "floor_type");

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

            cmd.CommandText = GenerateSpriteTableSqlWithoutRender(id, "pickable_items",
                "item_type", "quantity", "time_before_disapear");

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

            cmd.CommandText = GenerateSpriteTableSql(id, "gate", "activated");

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

            cmd.CommandText = GenerateSpriteTableSql(id, "rift", "lifepoints");

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
            cmd.CommandText = GenerateSpriteTableSql(id, "gate_trigger", otherColums.ToArray());

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

        // Generates SQL query to get informations about a certain type of sprite relatives to a screen.
        private string GenerateSpriteTableSql(int screenId, string table, params string[] additionalColumns)
        {
            return string.Format(
                "select x, y, width, height, render_type {0} {1} from {2} where screen_id = {3}",
                string.Concat(", ", string.Join(", ", GenerateSqlColumnsRender())),
                (additionalColumns?.Length > 0 ? ", " + string.Join(", ", additionalColumns) : string.Empty),
                table,
                screenId);
        }

        // Generates SQL query to get informations about a certain type of sprite relatives to a screen; no columns relatives to render.
        private string GenerateSpriteTableSqlWithoutRender(int screenId, string table, params string[] additionalColumns)
        {
            return string.Format(
                "select x, y, width, height {0} from {1} where screen_id = {2}",
                (additionalColumns?.Length > 0 ? ", " + string.Join(", ", additionalColumns) : string.Empty),
                table,
                screenId);
        }

        // Generates the SQL which contains every columns related to "render_value" (or similar pattern).
        private List<string> GenerateSqlColumnsRender(string pattern = "render_value")
        {
            List<string> cols = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                cols.Add(string.Format("{0}_{1}", pattern, i));
            }
            return cols;
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

    /// <summary>
    /// Extension methods for <see cref="SQLiteDataReader"/>.
    /// </summary>
    internal static class SQLiteDataReaderExtensions
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
    }
}
