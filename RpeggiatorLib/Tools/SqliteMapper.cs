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
        /// Number of columns used to store properties relatives to a render.
        /// </summary>
        public const int RENDER_COLUMNS_COUNT = 10;
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
                    using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = Properties.Resources.SpriteDb_sql;

                        cmd.ExecuteNonQuery();
                    }
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
                    
                    cmd.CommandText = GenerateSpriteTableSql("screen", true, new[]
                    {
                        "floor_type", "darkness_opacity", "neighboring_screen_top", "neighboring_screen_bottom",
                        "neighboring_screen_right", "neighboring_screen_left"
                    });
                    cmd.Parameters.Add("@id", DbType.Int32);
                    cmd.Parameters["@id"].Value = id;

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Dictionary<Enums.Direction, int> neighboringScreens = new Dictionary<Enums.Direction, int>
                            {
                                { Enums.Direction.Bottom, reader.GetInt32("neighboring_screen_bottom") },
                                { Enums.Direction.Top, reader.GetInt32("neighboring_screen_top") },
                                { Enums.Direction.Right, reader.GetInt32("neighboring_screen_right") },
                                { Enums.Direction.Left, reader.GetInt32("neighboring_screen_left") }
                            };

                            s = new Sprites.Screen(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                                reader.GetDouble("width"), reader.GetDouble("height"), (Enums.FloorType)reader.GetInt32("floor_type"),
                                reader.GetDouble("darkness_opacity"), reader.GetString("render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader), permanentStructures, doors, floors, enemies,
                                gateTriggers, gates, rifts, pits, chests, pickableItems, neighboringScreens);
                        }
                    }
                }
            }

            return s;
        }

        #region Static tool methods

        // Generates SQL query to get informations about a certain type of sprite relatives to a screen.
        private static string GenerateSpriteTableSql(string table, bool withRender, params string[] additionalColumns)
        {
            return string.Format(
                "select id, x, y, width, height {0} {1} from {2} where {3} = @{3}",
                withRender ? string.Concat(", render_type, ", string.Join(", ", GenerateSqlColumnsRender())) : string.Empty,
                (additionalColumns?.Length > 0 ? ", " + string.Join(", ", additionalColumns) : string.Empty),
                table, table == "screen" ? "id" : "screen_id");
        }

        // Generates the SQL which contains every columns related to "render_value" (or similar column base).
        private static List<string> GenerateSqlColumnsRender(string baseColumnName = "render_value")
        {
            List<string> cols = new List<string>();
            for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
            {
                cols.Add($"{baseColumnName}_{i}");
            }
            return cols;
        }

        // Builds an array of values relatives to the render
        private static object[] GetRenderPropertiesForCurrentReaderRow(SQLiteDataReader reader, string baseColumnName = "render_value")
        {
            object[] renderProperties = new object[RENDER_COLUMNS_COUNT];
            for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
            {
                renderProperties[i] = reader[$"{baseColumnName}_{i}"];
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
        private static void ExecutePreparedInsert(string tableName, string[] columns, DbType[] types, params object[][] rowsValues)
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

        // Creats an array with every column names to insert for a sprite.
        private static string[] ToColumnsArray(bool withRender, params string[] additionalColumns)
        {
            List<string> columns = new List<string>
            {
                "id", "screen_id", "x", "y", "width", "height"
            };
            if (withRender)
            {
                columns.Add("render_type");
                for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
                {
                    columns.Add(string.Join("render_value_{0}", i));
                }
            }
            if (additionalColumns?.Length > 0)
            {
                columns.AddRange(additionalColumns);
            }
            return columns.ToArray();
        }

        // Creats an array with every database types to insert for a sprite.
        private static DbType[] ToTypesArray(bool withRender, params DbType[] additionalTypes)
        {
            List<DbType> types = new List<DbType>
            {
                DbType.Int32, DbType.Int32, DbType.Double, DbType.Double, DbType.Double, DbType.Double
            };
            if (withRender)
            {
                types.Add(DbType.String);
                for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
                {
                    types.Add(DbType.String);
                }
            }
            if (additionalTypes?.Length > 0)
            {
                types.AddRange(additionalTypes);
            }
            return types.ToArray();
        }

        // Creates an array with every values to insert for a sprite.
        private static object[] ToValuesArray(int id, int screenId, double x, double y, double width, double height, string renderType, object[] renderValues, params object[] additionalValues)
        {
            List<object> values = new List<object> { id, screenId, x, y, width, height };
            if (renderType != null)
            {
                values.Add(renderType);
            }
            if (renderValues != null)
            {
                for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
                {
                    if (renderValues.Length <= i)
                    {
                        values.Add(renderValues[i] ?? DBNull.Value);
                    }
                    else
                    {
                        values.Add(DBNull.Value);
                    }
                }
            }
            foreach (object o in additionalValues)
            {
                if (o.GetType() == typeof(object[]))
                {
                    object[] oArray = o as object[];
                    for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
                    {
                        if (oArray.Length <= i)
                        {
                            values.Add(oArray[i] ?? DBNull.Value);
                        }
                        else
                        {
                            values.Add(DBNull.Value);
                        }
                    }
                }
                else
                {
                    values.Add(o ?? DBNull.Value);
                }
            }
            return values.ToArray();
        }

        // Gets the next identifier for the specified table.
        private static int GetNextId(string tableName)
        {
            int id = 1;
            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"select max(id) from {tableName}";
                    id += Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                }
            }

            return id;
        }

        #endregion

        #region Get screen informations by sprite type.

        // Gets permanent structures of a screen.
        private List<Sprites.PermanentStructure> GetPermanentStructures(int screnId, SQLiteCommand cmd)
        {
            List<Sprites.PermanentStructure> sprites = new List<Sprites.PermanentStructure>();

            cmd.CommandText = GenerateSpriteTableSql("permanent_structure", true, null);
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = screnId;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.PermanentStructure(
                        reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                        reader.GetDouble("height"), reader.GetString("render_type"), GetRenderPropertiesForCurrentReaderRow(reader)));
                }
            }

            return sprites;
        }

        // Gets doors of a screen.
        private List<Sprites.Door> GetDoors(int screenId, SQLiteCommand cmd)
        {
            List<Sprites.Door> sprites = new List<Sprites.Door>();

            List<string> otherColumns = new List<string>
            {
                "key_id", "connected_screen_id", "player_go_through_x", "player_go_through_y", "locked_render_type"
            };
            otherColumns.AddRange(GenerateSqlColumnsRender("locked_render_value"));
            cmd.CommandText = GenerateSpriteTableSql("door", true, otherColumns.ToArray());
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = screenId;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Door(
                        reader.GetInt32("id"), reader.GetDouble('x'), reader.GetDouble("y"), reader.GetDouble("width"),
                        reader.GetDouble("height"), reader.GetNullValue<int>("key_id"), reader.GetInt32("connected_screen_id"),
                        reader.GetDouble("player_go_through_x"), reader.GetDouble("player_go_through_y"),
                        reader.GetString("locked_render_type"), GetRenderPropertiesForCurrentReaderRow(reader, "locked_render_value"),
                        reader.GetString("render_type"), GetRenderPropertiesForCurrentReaderRow(reader)));
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

            cmd.CommandText = GenerateSpriteTableSql("chest", true, otherColumns.ToArray());
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Chest(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                        reader.GetDouble("width"), reader.GetDouble("height"), (Enums.ItemType?)reader.GetNullValue<int>("item_type"),
                        reader.GetInt32("quantity"), reader.GetNullValue<int>("key_id"), reader.GetNullValue<int>("key_id_container"),
                        reader.GetString("render_type"), GetRenderPropertiesForCurrentReaderRow(reader),
                        reader.GetString("open_render_type"), GetRenderPropertiesForCurrentReaderRow(reader, "open_render_value")));
                }
            }

            return sprites;
        }

        // Gets enemies of a screen.
        private List<Sprites.Enemy> GetEnemies(int id, SQLiteCommand cmd)
        {
            List<Sprites.Enemy> sprites = new List<Sprites.Enemy>();

            cmd.CommandText = GenerateSpriteTableSql("enemy", false, "maximal_life_points", "hit_life_point_cost", "speed",
                "recovery_time", "render_filename", "render_recovery_filename", "default_direction", "loot_item_type", "loot_quantity");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Enemy(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                        reader.GetDouble("width"), reader.GetDouble("height"), reader.GetDouble("maximal_life_points"),
                        reader.GetDouble("hit_life_point_cost"), reader.GetDouble("speed"), reader.GetDouble("recovery_time"),
                        reader.GetString("render_filename"), reader.GetString("render_recovery_filename"),
                        (Enums.Direction)reader.GetInt32("default_direction"),
                        (Enums.ItemType?)reader.GetNullValue<int>("loot_item_type"), reader.GetInt32("loot_quantity")));

                    sprites.Last().SetPath(GetEnemyPathSteps(reader.GetInt32(13)));
                }
            }

            return sprites;
        }

        // gets path steps for an enemy.
        private List<Point> GetEnemyPathSteps(int enemyId)
        {
            List<Point> pointOfSteps = new List<Point>();

            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "select x, y from enemy_step where enemy_id = @enemyId order by step_no asc";
                    cmd.Parameters.Add("@enemy_id", DbType.Int32);
                    cmd.Parameters["@enemy_id"].Value = enemyId;

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pointOfSteps.Add(new Point(reader.GetDouble(0), reader.GetDouble(1)));
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

            cmd.CommandText = GenerateSpriteTableSql("pit", true, "screen_id_entrance");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Pit(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                        reader.GetDouble("width"), reader.GetDouble("height"), reader.GetNullValue<int>("screen_id_entrance"),
                        reader.GetString("render_type"), GetRenderPropertiesForCurrentReaderRow(reader)));
                }
            }

            return sprites;
        }

        // Gets floors of a screen.
        private List<Sprites.Floor> GetFloors(int id, SQLiteCommand cmd)
        {
            List<Sprites.Floor> sprites = new List<Sprites.Floor>();

            cmd.CommandText = GenerateSpriteTableSql("floor", true, "floor_type");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Floor(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                        reader.GetDouble("width"), reader.GetDouble("height"), (Enums.FloorType)reader.GetInt32("floor_type"),
                        reader.GetString("render_type"), GetRenderPropertiesForCurrentReaderRow(reader)));
                }
            }

            return sprites;
        }

        // Gets pickable items of a screen.
        private List<Sprites.PickableItem> GetPickableItems(int id, SQLiteCommand cmd)
        {
            List<Sprites.PickableItem> sprites = new List<Sprites.PickableItem>();

            cmd.CommandText = GenerateSpriteTableSql("pickable_items", false,
                "item_type", "quantity", "time_before_disapear");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.PickableItem(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                        reader.GetDouble("width"), reader.GetDouble("height"), (Enums.ItemType?)reader.GetNullValue<int>("item_type"),
                        reader.GetInt32("quantity"), reader.GetNullValue<double>("time_before_disapear")));
                }
            }

            return sprites;
        }

        // Gets gates of a screen.
        private List<Sprites.Gate> GetGates(int id, SQLiteCommand cmd)
        {
            List<Sprites.Gate> sprites = new List<Sprites.Gate>();

            cmd.CommandText = GenerateSpriteTableSql("gate", true, "activated");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Gate(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), +
                        reader.GetDouble("width"), reader.GetDouble("height"), reader.GetInt32("activated") > 0,
                        reader.GetString("render_type"), GetRenderPropertiesForCurrentReaderRow(reader)));
                }
            }

            return sprites;
        }

        // Gets rifts of a screen.
        private List<Sprites.Rift> GetRifts(int id, SQLiteCommand cmd)
        {
            List<Sprites.Rift> sprites = new List<Sprites.Rift>();

            cmd.CommandText = GenerateSpriteTableSql("rift", true, "lifepoints");
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.Rift(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                        reader.GetDouble("width"), reader.GetDouble("height"), reader.GetDouble("lifepoints"),
                        reader.GetString("render_type"), GetRenderPropertiesForCurrentReaderRow(reader)));
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

            cmd.CommandText = GenerateSpriteTableSql("gate_trigger", true, otherColums.ToArray());
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = id;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sprites.Add(new Sprites.GateTrigger(
                        reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                        reader.GetDouble("height"), reader.GetDouble("action_duration"), reader.GetInt32("gate_id"),
                        reader.GetInt32("appear_on_activation") > 0, reader.GetString("render_type"),
                        GetRenderPropertiesForCurrentReaderRow(reader), reader.GetString("on_render_type"),
                        GetRenderPropertiesForCurrentReaderRow(reader, "on_render_value")));
                }
            }

            return sprites;
        }

        #endregion

        #region Insert methods

        /// <summary>
        /// Creates or recreates every steps on an <see cref="Sprites.Enemy"/> path.
        /// </summary>
        /// <param name="enemyId"><see cref="Sprites.Enemy"/> identifier.</param>
        /// <param name="points">List of coordinates; the key indicates the order.</param>
        /// <exception cref="ArgumentException"><paramref name="enemyId"/> is lower or equals to zero.</exception>
        public void CreateEnemyPathSteps(int enemyId, Dictionary<int, System.Windows.Point> points)
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
                        points.Select(kvp => new object[] { enemyId, kvp.Key, kvp.Value.X, kvp.Value.Y }).ToArray()
                    );
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="Sprites.PermanentStructure"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreatePermanentStructure(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues)
        {
            int id = GetNextId("permanent_structure");

            ExecutePreparedInsert("permanent_structure",
                ToColumnsArray(true),
                ToTypesArray(true),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.GateTrigger"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="actionDuration"><see cref="Sprites.FloorTrigger._actionDuration"/></param>
        /// <param name="appearOnActivation"><see cref="Sprites.GateTrigger.AppearOnActivation"/></param>
        /// <param name="gateId"><see cref="Sprites.GateTrigger.GateId"/></param>
        /// <param name="onRenderType"><see cref="Sprites.GateTrigger._renderOn"/> render type name.</param>
        /// <param name="onRenderValues">Values required to instanciate a <see cref="Sprites.GateTrigger._renderOn"/> (filename, color, and so forth).</param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreateGateTrigger(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues,
            double actionDuration, int gateId, bool appearOnActivation,
            string onRenderType, object[] onRenderValues)
        {
            int id = GetNextId("gate_trigger");

            List<string> otherColumns = new List<string>
            {
                "action_duration", "gate_id", "appear_on_activation", "on_render_type"
            };
            List<DbType> otherTypes = new List<DbType>
            {
                DbType.Double, DbType.Int32, DbType.Boolean, DbType.String
            };
            List<object> otherValues = new List<object>
            {
                actionDuration, gateId, appearOnActivation, onRenderType
            };
            for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
            {
                otherColumns.Add($"on_render_value_{i}");
                otherTypes.Add(DbType.String);
            }
            otherValues.Add(onRenderValues);

            ExecutePreparedInsert("gate_trigger",
                ToColumnsArray(true, otherColumns.ToArray()),
                ToTypesArray(true, otherTypes.ToArray()),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues, otherValues));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Rift"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="lifepoints"><see cref="Sprites.DamageableSprite.CurrentLifePoints"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreateRift(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues, double lifepoints)
        {
            int id = GetNextId("rift");

            ExecutePreparedInsert("rift",
                ToColumnsArray(true, "lifepoints"),
                ToTypesArray(true, DbType.Double),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues, lifepoints));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Gate"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="activated"><see cref="Sprites.Gate._defaultActivated"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreateGate(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues, bool activated)
        {
            int id = GetNextId("gate");

            ExecutePreparedInsert("gate",
                ToColumnsArray(true, "activated"),
                ToTypesArray(true, DbType.Boolean),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues, activated));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.PickableItem"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="itemType"><see cref="Sprites.PickableItem.ItemType"/></param>
        /// <param name="quantity"><see cref="Sprites.PickableItem.Quantity"/></param>
        /// <param name="timeBeforeDisapear">Duration, in milliseconds, before <see cref="Sprites.PickableItem.Disapear"/>.</param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreatePickableItem(int screenId, double x, double y, double width, double height,
            Enums.ItemType? itemType, int quantity, double? timeBeforeDisapear)
        {
            int id = GetNextId("pickable_item");

            ExecutePreparedInsert("pickable_item",
                   ToColumnsArray(true, "item_type", "quantity", "time_before_disapear"),
                   ToTypesArray(true, DbType.Int32, DbType.Int32, DbType.Double),
                   ToValuesArray(id, screenId, x, y, width, height, null, null, (int?)itemType, quantity, timeBeforeDisapear));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Door"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="connectedScreenId"><see cref="Sprites.Door._connectedScreenId"/></param>
        /// <param name="keyId"><see cref="Sprites.Door._keyId"/></param>
        /// <param name="lockedRenderType"><see cref="Sprites.Door._renderLocked"/> render type name.</param>
        /// <param name="lockedRenderValues">Values required to instanciate a <see cref="Sprites.Door._renderLocked"/> (filename, color, and so forth).</param>
        /// <param name="playerGoThroughX"><see cref="Sprites.Door.PlayerGoThroughX"/></param>
        /// <param name="playerGoThroughY"><see cref="Sprites.Door.PlayerGoThroughY"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreateDoor(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues,
            int keyId, int connectedScreenId, double playerGoThroughX, double playerGoThroughY,
            string lockedRenderType, object[] lockedRenderValues)
        {
            int id = GetNextId("door");

            List<string> otherColumns = new List<string>
            {
                "key_id", "connected_screen_id", "player_go_through_x", "player_go_through_y", "locked_render_type"
            };
            List<DbType> otherTypes = new List<DbType>
            {
                DbType.Int32, DbType.Int32, DbType.Double, DbType.Double, DbType.String
            };
            List<object> otherValues = new List<object>
            {
                keyId, connectedScreenId, playerGoThroughX, playerGoThroughY, lockedRenderType
            };
            for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
            {
                otherColumns.Add($"locked_render_value_{i}");
                otherTypes.Add(DbType.String);
            }
            otherValues.Add(lockedRenderValues);

            ExecutePreparedInsert("door",
                ToColumnsArray(true, otherColumns.ToArray()),
                ToTypesArray(true, otherTypes.ToArray()),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues, otherValues));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Floor"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="floorType"><see cref="Sprites.Floor.FloorType"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreateFloor(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues, Enums.FloorType floorType)
        {
            int id = GetNextId("floor");

            ExecutePreparedInsert("floor",
                ToColumnsArray(true, "floor_type"),
                ToTypesArray(true, DbType.Int32),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues, (int)floorType));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Pit"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="screenIdEntrance"><see cref="Sprites.Pit.ScreenIdEntrance"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreatePit(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues, int? screenIdEntrance)
        {
            int id = GetNextId("pit");

            ExecutePreparedInsert("pit",
                ToColumnsArray(true, "screen_id_entrance"),
                ToTypesArray(true, DbType.Int32),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues, screenIdEntrance));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Chest"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="itemType"><see cref="Sprites.Chest._itemType"/></param>
        /// <param name="keyId"><see cref="Sprites.Chest._keyId"/></param>
        /// <param name="openRenderType"><see cref="Sprites.Chest._renderOpen"/> render type name.</param>
        /// <param name="keyIdContainer"><see cref="Sprites.Chest._keyIdContainer"/></param>
        /// <param name="openRenderValues">Values required to instanciate a <see cref="Sprites.Chest._renderOpen"/> (filename, color, and so forth).</param>
        /// <param name="quantity"><see cref="Sprites.Chest._quantity"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreateChest(int screenId, double x, double y, double width, double height,
            string renderType, object[] renderValues,
            Enums.ItemType? itemType, int quantity, int? keyId, int? keyIdContainer, string openRenderType, object[] openRenderValues)
        {
            int id = GetNextId("chest");

            List<string> otherColumns = new List<string>
            {
                "item_type", "quantity", "key_id", "key_id_container", "open_render_type"
            };
            List<DbType> otherTypes = new List<DbType>
            {
                DbType.Int32, DbType.Int32, DbType.Int32, DbType.Int32, DbType.String
            };
            List<object> otherValues = new List<object>
            {
                itemType.HasValue ? (int?)itemType.Value : null, quantity, keyId, keyIdContainer, openRenderType
            };
            for (int i = 0; i < RENDER_COLUMNS_COUNT; i++)
            {
                otherColumns.Add($"open_render_value_{i}");
                otherTypes.Add(DbType.String);
            }
            otherValues.Add(openRenderValues);

            ExecutePreparedInsert("chest",
                ToColumnsArray(true, otherColumns.ToArray()),
                ToTypesArray(true, otherTypes.ToArray()),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValues, otherValues.ToArray()));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Enemy"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="defaultDirection"><see cref="Sprites.LifeSprite.Direction"/></param>
        /// <param name="hitLifePointCost"><see cref="Sprites.LifeSprite.HitLifePointCost"/></param>
        /// <param name="lootItemType"><see cref="Sprites.Enemy.LootItemType"/>; <c>Null</c> for coin.</param>
        /// <param name="lootQuantity"><see cref="Sprites.Enemy.LootQuantity"/></param>
        /// <param name="maximalLifePoints"><see cref="Sprites.DamageableSprite.CurrentLifePoints"/></param>
        /// <param name="renderFilename"><see cref="Sprites.Sprite.Render"/> image filename.</param>
        /// <param name="recoveryTime"><see cref="Sprites.LifeSprite._recoveryTime"/></param>
        /// <param name="renderRecoveryFilename"><see cref="Sprites.LifeSprite._renderRecovery"/> image filename.</param>
        /// <param name="speed"><see cref="Sprites.LifeSprite.Speed"/></param>
        /// <remarks><see cref="Sprites.Sprite.Id"/></remarks>
        public int CreateEnemy(int screenId, double x, double y, double width, double height, double maximalLifePoints,
            double hitLifePointCost, double speed, double recoveryTime, string renderFilename, string renderRecoveryFilename,
            Enums.Direction defaultDirection, Enums.ItemType? lootItemType, int lootQuantity)
        {
            int id = GetNextId("enemy");

            List<string> otherColumns = new List<string>
            {
                "maximal_life_points", "hit_life_point_cost", "speed", "recovery_time", "render_filename",
                "render_recovery_filename", "default_direction", "loot_item_type", "loot_quantity"
            };
            List<DbType> otherTypes = new List<DbType>
            {
                DbType.Double, DbType.Double, DbType.Double, DbType.Double, DbType.String,
                DbType.String, DbType.Int32, DbType.Int32, DbType.Int32
            };
            List<object> otherValues = new List<object>
            {
                maximalLifePoints, hitLifePointCost, speed, recoveryTime, renderFilename,
                renderRecoveryFilename, (int)defaultDirection, (int?)lootItemType, lootQuantity, id
            };

            ExecutePreparedInsert("enemy",
                ToColumnsArray(false, otherColumns.ToArray()),
                ToTypesArray(false, otherTypes.ToArray()),
                ToValuesArray(id, screenId, x, y, width, height, null, null, otherValues));

            return id;
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Screen"/> in the database.
        /// </summary>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="Sprites.Sprite.Render"/> render type name.</param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="darknessOpacity"><see cref="Sprites.Screen.DarknessOpacity"/></param>
        /// <param name="floorType"><see cref="Sprites.Floor.FloorType"/></param>
        /// <param name="neighboringScreenLeft"><see cref="Sprites.Screen._neighboringScreens"/></param>
        /// <param name="neighboringScreenBottom"><see cref="Sprites.Screen._neighboringScreens"/></param>
        /// <param name="neighboringScreenRight"><see cref="Sprites.Screen._neighboringScreens"/></param>
        /// <param name="neighboringScreenTop"><see cref="Sprites.Screen._neighboringScreens"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        public int CreateScreen(double x, double y, double width, double height, string renderType, object[] renderValues,
            Enums.FloorType floorType, double darknessOpacity, int neighboringScreenTop, int neighboringScreenBottom,
            int neighboringScreenRight, int neighboringScreenLeft)
        {
            int id = GetNextId("screen");

            List<string> columnsList = ToColumnsArray(true, "floor_type", "darkness_opacity", "neighboring_screen_top",
                "neighboring_screen_bottom", "neighboring_screen_right", "neighboring_screen_left").ToList();
            columnsList.RemoveAt(1);
            List<DbType> typesList = ToTypesArray(true, DbType.Int32, DbType.Double, DbType.Int32,
                DbType.Int32, DbType.Int32, DbType.Int32).ToList();
            typesList.RemoveAt(1);
            List<Object> valuesList = ToValuesArray(id, 0, x, y, width, height, renderType, renderValues, floorType,
                darknessOpacity, neighboringScreenTop, neighboringScreenBottom, neighboringScreenRight, neighboringScreenLeft).ToList();
            valuesList.RemoveAt(1);

            ExecutePreparedInsert("screen",
                columnsList.ToArray(),
                typesList.ToArray(),
                valuesList.ToArray());

            return id;
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
        /// <param name="columnName">Column name.</param>
        /// <returns>Value.</returns>
        internal static T? GetNullValue<T>(this SQLiteDataReader reader, string columnName) where T : struct
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? (T?)null
                : (T)Convert.ChangeType(reader[columnName], typeof(T));
        }

        /// <summary>
        /// Transforms a <see cref="string"/> which represents a SQL column, to its associated parameter name.
        /// </summary>
        internal static string ToParam(this string column)
        {
            return string.Concat("@", column);
        }

        /// <summary>
        /// Gets a value of type <see cref="bool"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="bool"/>.</returns>
        internal static bool GetBoolean(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetBoolean(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="byte"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="byte"/>.</returns>
        internal static byte GetByte(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetByte(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="DateTime"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="DateTime"/>.</returns>
        internal static DateTime GetDateTime(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetDateTime(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="decimal"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="decimal"/>.</returns>
        internal static decimal GetDecimal(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetDecimal(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="double"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="double"/>.</returns>
        internal static double GetDouble(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetDouble(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="float"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="float"/>.</returns>
        internal static float GetFloat(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetFloat(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="short"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="short"/>.</returns>
        internal static short GetInt16(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetInt16(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="int"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="int"/>.</returns>
        internal static int GetInt32(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetInt32(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets a value of type <see cref="long"/> from <paramref name="reader"/> at the specified <paramref name="columnName"/>.
        /// </summary>
        /// <param name="reader"><see cref="SQLiteDataReader"/></param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Value of type <see cref="long"/>.</returns>
        internal static long GetInt64(this SQLiteDataReader reader, string columnName)
        {
            return reader.GetInt64(reader.GetOrdinal(columnName));
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
