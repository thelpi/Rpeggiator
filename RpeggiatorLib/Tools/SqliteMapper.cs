using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using RpeggiatorLib.Enums;
using SysColor = System.Windows.Media.Colors;

namespace RpeggiatorLib
{
    /// <summary>
    /// Mapper for Sqlite database.
    /// </summary>
    public class SqliteMapper
    {
        // Path to resources.
        private readonly string _resourcePath = null;

        // Caracter used as separator in "render_velue" SQL column.
        private const char RENDER_VALUES_SEPARATOR = '|';
        // Escape string to substitute the caracter used as separator in "render_velue" SQL column.
        private const string RENDER_VALUES_ESCAPE_SEPARATOR = "ù!*^¨§µ^*!";
        // Database name.
        private const string DB_NAME = "SpriteDb.sqlite";
        // Connection string.
        private static readonly string CONN_STRING = string.Format("Data Source={0};Version=3;", DB_NAME);

        private static SqliteMapper _default = null;
        
        /// <summary>
        /// Default instance (singleton).
        /// </summary>
        /// <param name="resourcesPath">Path to resources.</param>
        public static SqliteMapper Defaut(string resourcesPath)
        {
            if (_default == null)
            {
                _default = new SqliteMapper(resourcesPath);
            }

            return _default;
        }

        // Private constructor.
        private SqliteMapper(string resourcesPath)
        {
            _resourcePath = resourcesPath;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(Messages.InvalidSqliteConnectionExceptionMessage, ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Clears the database and creates a new one.
        /// </summary>
        /// <param name="createDefaultDatas">Creates some default screens.</param>
        public void ResetDatabase(bool createDefaultDatas)
        {
            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = Properties.Resources.SpriteDb;

                    cmd.ExecuteNonQuery();
                }
            }

            if (createDefaultDatas)
            {
                GenerateDefaultDatas();
            }
        }

        // Generates default datas.
        private void GenerateDefaultDatas()
        {
            #region First screen

            int screenId1 = CreateScreen(RenderType.Plain, new object[] { SysColor.PapayaWhip }, FloorType.Ground, 0);
            CreatePermanentStructure(screenId1, 0, 300, 300, 50, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId1, 100, 260, 50, 140, RenderType.ImageMosaic, new object[] { "Tree" });
            CreateChest(screenId1, 400, 380, 40, 40, RenderType.Image, new object[] { "Chest" }, null, 10, null, 1, RenderType.Image, new object[] { "OpenChest" });
            CreatePit(screenId1, 700, 100, 50, 50, RenderType.Image, new object[] { "Pit" }, null);
            CreateRift(screenId1, 700, 400, 20, 100, RenderType.Plain, new object[] { SysColor.BurlyWood }, 8);
            int enemyId = CreateEnemy(screenId1, 50, 50, 40, 40, 20, 1, 150, 500, "Enemy", "EnemyRecovery", Direction.Right, null, 10);
            CreateEnemy(screenId1, 600, 50, 40, 40, 20, 1, 150, 500, "Enemy", "EnemyRecovery", Direction.Bottom, ItemType.Arrow, 1);
            CreateEnemyPathSteps(enemyId, new Dictionary<int, System.Windows.Point>
                {
                    { 1, new System.Windows.Point(730, 350) },
                    { 2, new System.Windows.Point(730, 400) },
                    { 3, new System.Windows.Point(750, 550) },
                    { 4, new System.Windows.Point(30, 520) },
                });

            #endregion

            #region Left screen

            int screenId2 = CreateScreen(RenderType.Plain, new object[] { SysColor.PapayaWhip }, FloorType.Ground, 0.8);
            CreatePermanentStructure(screenId2, 80, 80, 480, 40, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId2, 80, 80, 40, 320, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId2, 520, 80, 40, 320, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId2, 80, 360, 300, 40, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId2, 480, 360, 80, 40, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId2, 400, 590, 80, 10, RenderType.ImageMosaic, new object[] { "Tree" });
            int gateId = CreateGate(screenId2, 380, 360, 100, 40, RenderType.Plain, new object[] { SysColor.Gainsboro }, true);
            CreateGateTrigger(screenId2, 570, 20, 20, 20, RenderType.Image, new object[] { "TriggerOff" }, 5000, gateId, false, RenderType.Image, new object[] { "TriggerOn" });
            CreateGateTrigger(screenId2, 160, 220, 20, 20, RenderType.Image, new object[] { "TriggerOff" }, 5000, gateId, false, RenderType.Image, new object[] { "TriggerOn" });
            CreatePickableItem(screenId2, 310, 230, Constants.Bomb.WIDTH, Constants.Bomb.HEIGHT, ItemType.Bomb, 10, null);
            CreatePit(screenId2, 600, 400, 50, 50, RenderType.Image, new object[] { "Pit" }, screenId1);

            #endregion

            #region Top screen

            int screenId3 = CreateScreen(RenderType.Plain, new object[] { SysColor.PapayaWhip }, FloorType.Ground, 0);
            CreateFloor(screenId3, 150, 150, 300, 200, RenderType.Plain, new object[] { SysColor.Blue }, FloorType.Water);
            CreateFloor(screenId3, 460, 150, 150, 200, RenderType.Plain, new object[] { SysColor.Crimson }, FloorType.Lava);
            CreateFloor(screenId3, 200, 360, 400, 100, RenderType.Plain, new object[] { SysColor.Azure }, FloorType.Ice);
            CreatePermanentStructure(screenId3, 0, 0, 800, 20, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId3, 0, 20, 20, 560, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId3, 780, 20, 20, 560, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId3, 0, 580, 400, 20, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId3, 480, 580, 320, 20, RenderType.ImageMosaic, new object[] { "Tree" });
            CreateDoor(screenId3, 400, 580, 80, 20, RenderType.Image, new object[] { "Door" }, 1, screenId1, 400, 20, RenderType.Image, new object[] { "DoorLocked" });

            #endregion

            #region Right screen

            int screenId4 = CreateScreen(RenderType.Plain, new object[] { SysColor.PapayaWhip }, FloorType.Ground, 0);
            CreatePermanentStructure(screenId4, 166, 49, 495, 50, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId4, 63, 50, 49, 194, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId4, 629, 236, 42, 284, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId4, 63, 327, 434, 61, RenderType.ImageMosaic, new object[] { "Tree" });
            CreatePermanentStructure(screenId4, 69, 469, 407, 56, RenderType.ImageMosaic, new object[] { "Tree" });

            #endregion

            SetNeighboringScreens(screenId1, screenId2, screenId2, screenId4, screenId2);
            SetNeighboringScreens(screenId2, screenId1, screenId1, screenId1, screenId1);
            SetNeighboringScreens(screenId3, screenId1, screenId1, screenId1, screenId1);
            SetNeighboringScreens(screenId4, screenId1, screenId1, screenId1, screenId1);

            // Screen ID 3 is required to create this door.
            CreateDoor(screenId1, 400, 0, 80, 20, RenderType.Image, new[] { "Door" }, 1, screenId3, 400, 540, RenderType.Image, new[] { "DoorLocked" });
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
                    List<Sprites.PermanentStructure>  permanentStructures = GetTypedSpritesForAScreen<Sprites.PermanentStructure>(id, cmd);
                    List<Sprites.Door> doors = GetTypedSpritesForAScreen<Sprites.Door>(id, cmd);
                    List<Sprites.Floor> floors = GetTypedSpritesForAScreen<Sprites.Floor>(id, cmd);
                    List<Sprites.Enemy> enemies = GetTypedSpritesForAScreen<Sprites.Enemy>(id, cmd);
                    enemies.ForEach(e => e.SetPath(GetEnemyPathSteps(e.Id)));
                    List<Sprites.GateTrigger> gateTriggers = GetTypedSpritesForAScreen<Sprites.GateTrigger>(id, cmd);
                    List<Sprites.Gate> gates = GetTypedSpritesForAScreen<Sprites.Gate>(id, cmd);
                    List<Sprites.Rift> rifts = GetTypedSpritesForAScreen<Sprites.Rift>(id, cmd);
                    List<Sprites.Pit> pits = GetTypedSpritesForAScreen<Sprites.Pit>(id, cmd);
                    List<Sprites.Chest> chests = GetTypedSpritesForAScreen<Sprites.Chest>(id, cmd);
                    List<Sprites.PickableItem> pickableItems = GetTypedSpritesForAScreen<Sprites.PickableItem>(id, cmd);
                    
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
                            Dictionary<Direction, int> neighboringScreens = new Dictionary<Direction, int>
                            {
                                { Direction.Bottom, reader.GetInt32("neighboring_screen_bottom") },
                                { Direction.Top, reader.GetInt32("neighboring_screen_top") },
                                { Direction.Right, reader.GetInt32("neighboring_screen_right") },
                                { Direction.Left, reader.GetInt32("neighboring_screen_left") }
                            };

                            s = new Sprites.Screen(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                                reader.GetDouble("width"), reader.GetDouble("height"), (FloorType)reader.GetInt32("floor_type"),
                                reader.GetDouble("darkness_opacity"), (RenderType)reader.GetInt32("render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader), permanentStructures, doors, floors, enemies,
                                gateTriggers, gates, rifts, pits, chests, pickableItems, neighboringScreens);
                        }
                    }
                }
            }

            return s;
        }

        // Gets, for a specified screen, every sprites of a specified type.
        private List<T> GetTypedSpritesForAScreen<T>(int screenId, SQLiteCommand cmd) where T : Sprites.Sprite
        {
            string tableName = "";
            bool withRender = true;
            string[] additionalColumns = null;
            switch (typeof(T).Name)
            {
                case nameof(Sprites.Chest):
                    tableName = "chest";
                    additionalColumns = new[] { "item_type", "quantity", "key_id", "key_id_container", "open_render_type", "open_render_value" };
                    break;
                case nameof(Sprites.Door):
                    tableName = "door";
                    additionalColumns = new[] { "key_id", "connected_screen_id", "player_go_through_x", "player_go_through_y",
                        "locked_render_type", "locked_render_value" };
                    break;
                case nameof(Sprites.Enemy):
                    tableName = "enemy";
                    withRender = false;
                    additionalColumns = new[] { "maximal_life_points", "hit_life_point_cost", "speed", "recovery_time", "render_filename",
                        "render_recovery_filename", "default_direction", "loot_item_type", "loot_quantity" };
                    break;
                case nameof(Sprites.Floor):
                    tableName = "floor";
                    additionalColumns = new[] { "floor_type" };
                    break;
                case nameof(Sprites.Gate):
                    tableName = "gate";
                    additionalColumns = new[] { "activated" };
                    break;
                case nameof(Sprites.GateTrigger):
                    tableName = "gate_trigger";
                    additionalColumns = new[] { "action_duration", "gate_id", "appear_on_activation", "on_render_type", "on_render_value" };
                    break;
                case nameof(Sprites.PermanentStructure):
                    tableName = "permanent_structure";
                    break;
                case nameof(Sprites.PickableItem):
                    tableName = "pickable_item";
                    withRender = false;
                    additionalColumns = new[] { "item_type", "quantity", "time_before_disapear" };
                    break;
                case nameof(Sprites.Pit):
                    tableName = "pit";
                    additionalColumns = new[] { "screen_id_entrance" };
                    break;
                case nameof(Sprites.Rift):
                    tableName = "rift";
                    additionalColumns = new[] { "lifepoints" };
                    break;
                default:
                    throw new NotImplementedException();
            }

            List<T> sprites = new List<T>();

            cmd.CommandText = GenerateSpriteTableSql(tableName, withRender, additionalColumns);
            cmd.Parameters.Add("@screen_id", DbType.Int32);
            cmd.Parameters["@screen_id"].Value = screenId;

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Sprites.Sprite sp;
                    switch (typeof(T).Name)
                    {
                        case nameof(Sprites.Chest):
                            sp = new Sprites.Chest(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                                reader.GetDouble("height"), (ItemType?)reader.GetNullValue<int>("item_type"), reader.GetInt32("quantity"),
                                reader.GetNullValue<int>("key_id"), reader.GetNullValue<int>("key_id_container"),
                                (RenderType)reader.GetInt32("render_type"), GetRenderPropertiesForCurrentReaderRow(reader),
                                (RenderType)reader.GetInt32("open_render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader, "open_render_value"));
                            break;
                        case nameof(Sprites.Door):
                            sp = new Sprites.Door(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                                reader.GetDouble("height"), reader.GetNullValue<int>("key_id"), reader.GetInt32("connected_screen_id"),
                                reader.GetDouble("player_go_through_x"), reader.GetDouble("player_go_through_y"),
                                (RenderType)reader.GetInt32("locked_render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader, "locked_render_value"),
                                (RenderType)reader.GetInt32("render_type"), GetRenderPropertiesForCurrentReaderRow(reader));
                            break;
                        case nameof(Sprites.Enemy):
                            sp = new Sprites.Enemy(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                                reader.GetDouble("height"), reader.GetDouble("maximal_life_points"), reader.GetDouble("hit_life_point_cost"),
                                reader.GetDouble("speed"), reader.GetDouble("recovery_time"), reader.GetString("render_filename"),
                                reader.GetString("render_recovery_filename"), (Direction)reader.GetInt32("default_direction"),
                                (ItemType?)reader.GetNullValue<int>("loot_item_type"), reader.GetInt32("loot_quantity"));
                            break;
                        case nameof(Sprites.Floor):
                            sp = new Sprites.Floor(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                                reader.GetDouble("height"), (FloorType)reader.GetInt32("floor_type"),
                                (RenderType)reader.GetInt32("render_type"), GetRenderPropertiesForCurrentReaderRow(reader));
                            break;
                        case nameof(Sprites.Gate):
                            sp = new Sprites.Gate(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                                reader.GetDouble("height"), reader.GetInt32("activated") > 0, (RenderType)reader.GetInt32("render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader));
                            break;
                        case nameof(Sprites.GateTrigger):
                            sp = new Sprites.GateTrigger(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                                reader.GetDouble("width"), reader.GetDouble("height"), reader.GetDouble("action_duration"),
                                reader.GetInt32("gate_id"), reader.GetInt32("appear_on_activation") > 0,
                                (RenderType)reader.GetInt32("render_type"), GetRenderPropertiesForCurrentReaderRow(reader),
                                (RenderType)reader.GetInt32("on_render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader, "on_render_value"));
                            break;
                        case nameof(Sprites.PermanentStructure):
                            sp = new Sprites.PermanentStructure(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                                reader.GetDouble("width"), reader.GetDouble("height"), (RenderType)reader.GetInt32("render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader));
                            break;
                        case nameof(Sprites.PickableItem):
                            sp = new Sprites.PickableItem(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"),
                                reader.GetDouble("width"), reader.GetDouble("height"), (ItemType?)reader.GetNullValue<int>("item_type"),
                                reader.GetInt32("quantity"), reader.GetNullValue<double>("time_before_disapear"));
                            break;
                        case nameof(Sprites.Pit):
                            sp = new Sprites.Pit(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                                reader.GetDouble("height"), reader.GetNullValue<int>("screen_id_entrance"),
                                (RenderType)reader.GetInt32("render_type"), GetRenderPropertiesForCurrentReaderRow(reader));
                            break;
                        case nameof(Sprites.Rift):
                            sp = new Sprites.Rift(reader.GetInt32("id"), reader.GetDouble("x"), reader.GetDouble("y"), reader.GetDouble("width"),
                                reader.GetDouble("height"), reader.GetDouble("lifepoints"), (RenderType)reader.GetInt32("render_type"),
                                GetRenderPropertiesForCurrentReaderRow(reader));
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    sprites.Add(sp as T);
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
                    cmd.CommandText = "select x, y from enemy_step where enemy_id = @enemy_id order by step_no asc";
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

        // Gets the next identifier for the specified table.
        private int GetNextId(string tableName)
        {
            int id = 1;
            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"select max(id) from {tableName}";
                    object baseScalarValue = cmd.ExecuteScalar();
                    id += Convert.ToInt32(baseScalarValue == null || baseScalarValue == DBNull.Value ? 0 : baseScalarValue);
                }
            }

            return id;
        }

        // Checks if the specified identifier exists for the specified table.
        private bool ExistsId(string tableName, int id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = string.Format($"select id from {tableName} where id = @id");
                    cmd.Parameters.Add("@id", DbType.Int32);
                    cmd.Parameters["@id"].Value = id;
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
        }

        // Creats a sprite of the specified type inside the specified screen.
        private int CreateSpriteInScreen<T>(string tableName, int screenId, double x, double y, double width, double height,
            RenderType? renderType, object[] renderValues, params Tuple<string, DbType, object>[] otherValues) where T : Sprites.Sprite
        {
            if (!ExistsId("screen", screenId))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(screenId));
            }
            CheckInputDimensions(x, y, width, height);
            string[] renderValueString = null;
            if (renderType.HasValue)
            {
                renderValueString = CheckRenderValues(renderType.Value, renderValues);
            }

            int id = GetNextId(tableName);

            ExecutePreparedInsert(tableName,
                ToColumnsArray(renderType.HasValue, otherValues.Select(t => t.Item1).ToArray()),
                ToTypesArray(renderType.HasValue, otherValues.Select(t => t.Item2).ToArray()),
                ToValuesArray(id, screenId, x, y, width, height, renderType, renderValueString, otherValues.Select(t => t.Item3).ToArray()));

            return id;
        }

        #region Static tool methods

        // Generates SQL query to get informations about a certain type of sprite relatives to a screen.
        private static string GenerateSpriteTableSql(string table, bool withRender, params string[] additionalColumns)
        {
            return string.Format(
                "select id, x, y, width, height {0} {1} from {2} where {3} = @{3}",
                withRender ? ", render_type, render_value" : string.Empty,
                (additionalColumns?.Length > 0 ? ", " + string.Join(", ", additionalColumns) : string.Empty),
                table, table == "screen" ? "id" : "screen_id");
        }

        // Builds an array of values relatives to the default render or a specified one.
        private static string[] GetRenderPropertiesForCurrentReaderRow(SQLiteDataReader reader, string columnName = "render_value")
        {
            return reader.GetString(columnName)
                    .Split(RENDER_VALUES_SEPARATOR)
                    .Select(v =>
                        v.Replace(RENDER_VALUES_ESCAPE_SEPARATOR, RENDER_VALUES_SEPARATOR.ToString()))
                    .ToArray();
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
                        cmd.Parameters.Add(string.Concat("@", columns[i]), types[i]);
                    }
                    cmd.Prepare();

                    foreach (object[] rowValue in rowsValues)
                    {
                        for (int i = 0; i < columns.Length; i++)
                        {
                            cmd.Parameters[string.Concat("@", columns[i])].Value = rowValue[i];
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
                columns.Add("render_value");
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
                types.Add(DbType.Int32);
                types.Add(DbType.String);
            }
            if (additionalTypes?.Length > 0)
            {
                types.AddRange(additionalTypes);
            }
            return types.ToArray();
        }

        // Creates an array with every values to insert for a sprite.
        private static object[] ToValuesArray(int id, int screenId, double x, double y, double width, double height, RenderType? renderType, string[] renderValues, params object[] additionalValues)
        {
            List<object> values = new List<object> { id, screenId, x, y, width, height };
            if (renderType.HasValue)
            {
                values.Add((int)renderType.Value);
                values.Add(RenderPropertiesToSqlValue(renderValues));
            }
            foreach (object o in additionalValues)
            {
                if (o?.GetType() == typeof(string[]))
                {
                    values.Add(RenderPropertiesToSqlValue(o as string[]));
                }
                else
                {
                    values.Add(o ?? DBNull.Value);
                }
            }
            return values.ToArray();
        }

        // Checks input dimensions and throws an exception if invalid.
        private static void CheckInputDimensions(double x, double y, double width, double height)
        {
            if (x.Lower(0))
            {
                throw new ArgumentException(Messages.InvalidDimensionForInsertionExceptionMessage, nameof(x));
            }
            if (y.Lower(0))
            {
                throw new ArgumentException(Messages.InvalidDimensionForInsertionExceptionMessage, nameof(y));
            }
            if (width.Lower(0) || (x + width).Greater(Constants.SCREEN_WIDTH))
            {
                throw new ArgumentException(Messages.InvalidDimensionForInsertionExceptionMessage, nameof(width));
            }
            if (height.Lower(0) || (y + height).Greater(Constants.SCREEN_HEIGHT))
            {
                throw new ArgumentException(Messages.InvalidDimensionForInsertionExceptionMessage, nameof(height));
            }
        }

        // Transforms an array of render properties to a string for SQL insertion.
        private static string RenderPropertiesToSqlValue(string[] renderValues)
        {
            return string.Join(
                RENDER_VALUES_SEPARATOR.ToString(),
                (renderValues ?? new string[0])
                    .Select(v =>
                        v.Replace(RENDER_VALUES_SEPARATOR.ToString(), RENDER_VALUES_ESCAPE_SEPARATOR)
                    )
            );
        }

        #endregion

        #region Render values check

        // Checks render properties and throws an exception if invalid.
        private string[] CheckRenderValues(RenderType renderType, object[] renderValues)
        {
            switch (renderType)
            {
                case RenderType.Plain:
                    CheckRenderValuesCount(renderValues, 1);
                    CheckRenderColor(renderValues, 0);
                    return new string[] { Tools.HexFromColor((System.Windows.Media.Color)renderValues[0]) };
                case RenderType.Image:
                case RenderType.ImageMosaic:
                    CheckRenderValuesCount(renderValues, 1);
                    CheckRenderFileName(renderValues, 0);
                    return new string[] { renderValues[0].ToString() };
                case RenderType.ImageAnimated:
                case RenderType.ImageMosaicAnimated:
                    CheckRenderValuesCount(renderValues, 3);
                    CheckRenderFileName(renderValues, 0, true);
                    CheckRenderElapserProperty(renderValues, 1);
                    CheckRenderElapserNextStep(renderValues, 2);
                    return new string[] { renderValues[0].ToString(), renderValues[1].ToString(), renderValues[2].ToString() };
                default:
                    throw new NotImplementedException(Messages.NotImplementedRenderExceptionMessage);
            }
        }

        private void CheckRenderValuesCount(object[] renderValues, int expectedCount)
        {
            if (renderValues == null || renderValues.Length < expectedCount)
            {
                throw new ArgumentException(Messages.InvalidRenderExceptionMessage, nameof(renderValues));
            }
        }

        private void CheckRenderColor(object[] renderValues, int index)
        {
            if (renderValues[index].GetType() != typeof(System.Windows.Media.Color))
            {
                throw new ArgumentException(Messages.InvalidRenderExceptionMessage, nameof(renderValues));
            }
        }

        private void CheckRenderFileName(object[] renderValues, int index, bool checkZeroSuffix = false)
        {
            string fullFilenameWithoutExtension = string.Concat(renderValues[index].ToString(), checkZeroSuffix ? "0" : string.Empty);
            if (renderValues[index] == null || !System.IO.File.Exists(Tools.GetImagePath(_resourcePath, fullFilenameWithoutExtension)))
            {
                throw new ArgumentException(Messages.InvalidRenderExceptionMessage, nameof(renderValues));
            }
        }

        private void CheckRenderElapserNextStep(object[] renderValues, int index)
        {
            if (renderValues[index] == null
                || renderValues[index].GetType() != typeof(double)
                || ((double)renderValues[index]).LowerEqual(0))
            {
                throw new ArgumentException(Messages.InvalidRenderExceptionMessage, nameof(renderValues));
            }
        }

        private void CheckRenderElapserProperty(object[] renderValues, int index)
        {
            if (renderValues[index] == null
                || renderValues[index].GetType() != typeof(int)
                || Enum.IsDefined(typeof(ElapserUse), renderValues[index]))
            {
                throw new ArgumentException(Messages.InvalidRenderExceptionMessage, nameof(renderValues));
            }
        }

        #endregion

        #region Public insertion methods

        /// <summary>
        /// Creates or recreates every steps on an <see cref="Sprites.Enemy"/> path.
        /// </summary>
        /// <param name="enemyId"><see cref="Sprites.Enemy"/> identifier.</param>
        /// <param name="points">List of coordinates; the key indicates the order.</param>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidPathStepCoordinatesExceptionMessage"/></exception>
        public void CreateEnemyPathSteps(int enemyId, Dictionary<int, System.Windows.Point> points)
        {
            if (!ExistsId("enemy", enemyId))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(enemyId));
            }

            if (points != null)
            {
                foreach (int key in points.Keys)
                {
                    if (points[key].X.Lower(0) || points[key].X.Greater(Constants.SCREEN_WIDTH)
                        || points[key].Y.Lower(0) || points[key].Y.Greater(Constants.SCREEN_HEIGHT))
                    {
                        throw new ArgumentException(Messages.InvalidPathStepCoordinatesExceptionMessage, nameof(points));
                    }
                }
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
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreatePermanentStructure(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues)
        {
            return CreateSpriteInScreen<Sprites.PermanentStructure>("permanent_structure", screenId, x, y, width, height, renderType, renderValues);
        }

        /// <summary>
        /// Creates a <see cref="Sprites.GateTrigger"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="actionDuration"><see cref="Sprites.FloorTrigger._actionDuration"/></param>
        /// <param name="appearOnActivation"><see cref="Sprites.GateTrigger.AppearOnActivation"/></param>
        /// <param name="gateId"><see cref="Sprites.GateTrigger.GateId"/></param>
        /// <param name="onRenderType"><see cref="RenderType"/></param>
        /// <param name="onRenderValues">Values required to instanciate a <see cref="Sprites.GateTrigger._renderOn"/> (filename, color, and so forth).</param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.LowerOrEqualZeroExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreateGateTrigger(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues,
            double actionDuration, int gateId, bool appearOnActivation,
            RenderType onRenderType, object[] onRenderValues)
        {
            if (actionDuration.LowerEqual(0))
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(actionDuration));
            }
            if (!ExistsId("gate", gateId))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(gateId));
            }
            string[] onRenderValuesString = CheckRenderValues(onRenderType, onRenderValues);

            return CreateSpriteInScreen<Sprites.GateTrigger>("gate_trigger", screenId, x, y, width, height, renderType, renderValues,
                new Tuple<string, DbType, object>("action_duration", DbType.Double, actionDuration),
                new Tuple<string, DbType, object>("gate_id", DbType.Int32, gateId),
                new Tuple<string, DbType, object>("appear_on_activation", DbType.Int32, appearOnActivation ? 1 : 0),
                new Tuple<string, DbType, object>("on_render_type", DbType.Int32, (int)onRenderType),
                new Tuple<string, DbType, object>("on_render_value", DbType.String, onRenderValuesString));
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Rift"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="lifepoints"><see cref="Sprites.DamageableSprite.CurrentLifePoints"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.LowerOrEqualZeroExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreateRift(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues, double lifepoints)
        {
            if (lifepoints.LowerEqual(0))
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(lifepoints));
            }

            return CreateSpriteInScreen<Sprites.Rift>("rift", screenId, x, y, width, height, renderType, renderValues,
                new Tuple<string, DbType, object>("lifepoints", DbType.Double, lifepoints));
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Gate"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="activated"><see cref="Sprites.Gate._defaultActivated"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreateGate(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues, bool activated)
        {
            return CreateSpriteInScreen<Sprites.Gate>("gate", screenId, x, y, width, height, renderType, renderValues,
                new Tuple<string, DbType, object>("activated", DbType.Int32, activated ? 1 : 0));
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
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.LowerOrEqualZeroExceptionMessage"/></exception>
        public int CreatePickableItem(int screenId, double x, double y, double width, double height,
            ItemType? itemType, int quantity, double? timeBeforeDisapear)
        {
            if (timeBeforeDisapear.HasValue && timeBeforeDisapear.Value.LowerEqual(0))
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(timeBeforeDisapear));
            }
            if (quantity <= 0)
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(quantity));
            }

            return CreateSpriteInScreen<Sprites.PickableItem>("pickable_item", screenId, x, y, width, height, null, null,
                new Tuple<string, DbType, object>("item_type", DbType.Int32, (int?)itemType),
                new Tuple<string, DbType, object>("quantity", DbType.Int32, quantity),
                new Tuple<string, DbType, object>("time_before_disapear", DbType.Double, timeBeforeDisapear));
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Door"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="connectedScreenId"><see cref="Sprites.Door._connectedScreenId"/></param>
        /// <param name="keyId"><see cref="Sprites.Door._keyId"/></param>
        /// <param name="lockedRenderType"><see cref="RenderType"/></param>
        /// <param name="lockedRenderValues">Values required to instanciate a <see cref="Sprites.Door._renderLocked"/> (filename, color, and so forth).</param>
        /// <param name="playerGoThroughX"><see cref="Sprites.Door.PlayerGoThroughX"/></param>
        /// <param name="playerGoThroughY"><see cref="Sprites.Door.PlayerGoThroughY"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.LowerOrEqualZeroExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidPlayerThroughDoorCoordinatesExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreateDoor(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues,
            int? keyId, int connectedScreenId, double playerGoThroughX, double playerGoThroughY,
            RenderType lockedRenderType, object[] lockedRenderValues)
        {
            if (keyId.HasValue && keyId <= 0)
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(keyId));
            }
            if (!ExistsId("screen", connectedScreenId))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(connectedScreenId));
            }
            if (playerGoThroughX.Lower(0) || playerGoThroughX.Greater(Constants.SCREEN_WIDTH - Constants.Player.SPRITE_WIDTH))
            {
                throw new ArgumentException(Messages.InvalidPlayerThroughDoorCoordinatesExceptionMessage, nameof(playerGoThroughX));
            }
            if (playerGoThroughY.Lower(0) || playerGoThroughY.Greater(Constants.SCREEN_HEIGHT - Constants.Player.SPRITE_HEIGHT))
            {
                throw new ArgumentException(Messages.InvalidPlayerThroughDoorCoordinatesExceptionMessage, nameof(playerGoThroughY));
            }
            string[] lockedRenderValuesString = CheckRenderValues(lockedRenderType, lockedRenderValues);

            return CreateSpriteInScreen<Sprites.Door>("door", screenId, x, y, width, height, renderType, renderValues,
                new Tuple<string, DbType, object>("key_id", DbType.Int32, keyId),
                new Tuple<string, DbType, object>("connected_screen_id", DbType.Int32, connectedScreenId),
                new Tuple<string, DbType, object>("player_go_through_x", DbType.Double, playerGoThroughX),
                new Tuple<string, DbType, object>("player_go_through_y", DbType.Double, playerGoThroughY),
                new Tuple<string, DbType, object>("locked_render_type", DbType.Int32, (int)lockedRenderType),
                new Tuple<string, DbType, object>("locked_render_value", DbType.String, lockedRenderValuesString));
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Floor"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="floorType"><see cref="Sprites.Floor.FloorType"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreateFloor(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues, FloorType floorType)
        {
            return CreateSpriteInScreen<Sprites.Floor>("floor", screenId, x, y, width, height, renderType, renderValues,
                new Tuple<string, DbType, object>("floor_type", DbType.Int32, (int)floorType));
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Pit"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="screenIdEntrance"><see cref="Sprites.Pit.ScreenIdEntrance"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreatePit(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues, int? screenIdEntrance)
        {
            if (screenIdEntrance.HasValue && !ExistsId("screen", screenIdEntrance.Value))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(screenIdEntrance));
            }

            return CreateSpriteInScreen<Sprites.Pit>("pit", screenId, x, y, width, height, renderType, renderValues,
                new Tuple<string, DbType, object>("screen_id_entrance", DbType.Int32, screenIdEntrance));
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Chest"/> in the database.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Screen"/> identifier.</param>
        /// <param name="x"><see cref="Sprites.Sprite.X"/></param>
        /// <param name="y"><see cref="Sprites.Sprite.Y"/></param>
        /// <param name="width"><see cref="Sprites.Sprite.Width"/></param>
        /// <param name="height"><see cref="Sprites.Sprite.Height"/></param>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="itemType"><see cref="Sprites.Chest._itemType"/></param>
        /// <param name="keyId"><see cref="Sprites.Chest._keyId"/></param>
        /// <param name="openRenderType"><see cref="RenderType"/></param>
        /// <param name="keyIdContainer"><see cref="Sprites.Chest._keyIdContainer"/></param>
        /// <param name="openRenderValues">Values required to instanciate a <see cref="Sprites.Chest._renderOpen"/> (filename, color, and so forth).</param>
        /// <param name="quantity"><see cref="Sprites.Chest._quantity"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.LowerOrEqualZeroExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreateChest(int screenId, double x, double y, double width, double height,
            RenderType renderType, object[] renderValues, ItemType? itemType, int quantity, int? keyId, int? keyIdContainer,
            RenderType openRenderType, object[] openRenderValues)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(quantity));
            }
            if (keyIdContainer.HasValue && keyIdContainer.Value <= 0)
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(keyIdContainer));
            }
            if (keyId.HasValue && keyId.Value <= 0)
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(keyId));
            }
            string[] openRenderValuesString = CheckRenderValues(openRenderType, openRenderValues);

            // The chest can't contain both item and key.
            if (keyIdContainer.HasValue)
            {
                itemType = null;
            }

            return CreateSpriteInScreen<Sprites.Chest>("chest", screenId, x, y, width, height, renderType, renderValues,
                new Tuple<string, DbType, object>("item_type", DbType.Int32, (int?)itemType),
                new Tuple<string, DbType, object>("quantity", DbType.Int32, quantity),
                new Tuple<string, DbType, object>("key_id", DbType.Int32, keyId),
                new Tuple<string, DbType, object>("key_id_container", DbType.Int32, keyIdContainer),
                new Tuple<string, DbType, object>("open_render_type", DbType.Int32, (int)openRenderType),
                new Tuple<string, DbType, object>("open_render_value", DbType.String, openRenderValuesString));
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
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidDimensionForInsertionExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.LowerOrEqualZeroExceptionMessage"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        public int CreateEnemy(int screenId, double x, double y, double width, double height, double maximalLifePoints,
            double hitLifePointCost, double speed, double recoveryTime, string renderFilename, string renderRecoveryFilename,
            Direction defaultDirection, ItemType? lootItemType, int lootQuantity)
        {
            if (maximalLifePoints.LowerEqual(0))
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(maximalLifePoints));
            }
            if (speed.LowerEqual(0))
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(speed));
            }
            if (hitLifePointCost.LowerEqual(0))
            {
                throw new ArgumentException(Messages.LowerOrEqualZeroExceptionMessage, nameof(hitLifePointCost));
            }
            if (string.IsNullOrWhiteSpace(renderFilename) || !System.IO.File.Exists(Tools.GetImagePath(_resourcePath, renderFilename)))
            {
                throw new ArgumentException(Messages.InvalidRenderExceptionMessage, nameof(renderFilename));
            }
            if (string.IsNullOrWhiteSpace(renderRecoveryFilename) || !System.IO.File.Exists(Tools.GetImagePath(_resourcePath, string.Concat(renderRecoveryFilename, 0))))
            {
                throw new ArgumentException(Messages.InvalidRenderExceptionMessage, nameof(renderRecoveryFilename));
            }
            recoveryTime = recoveryTime < 0 ? 0 : recoveryTime;
            lootQuantity = lootQuantity < 0 ? 0 : lootQuantity;

            return CreateSpriteInScreen<Sprites.Enemy>("enemy", screenId, x, y, width, height, null, null,
                new Tuple<string, DbType, object>("maximal_life_points", DbType.Double, maximalLifePoints),
                new Tuple<string, DbType, object>("hit_life_point_cost", DbType.Double, hitLifePointCost),
                new Tuple<string, DbType, object>("speed", DbType.Double, speed),
                new Tuple<string, DbType, object>("recovery_time", DbType.Double, recoveryTime),
                new Tuple<string, DbType, object>("render_filename", DbType.String, renderFilename),
                new Tuple<string, DbType, object>("render_recovery_filename", DbType.String, renderRecoveryFilename),
                new Tuple<string, DbType, object>("default_direction", DbType.Int32, (int)defaultDirection),
                new Tuple<string, DbType, object>("loot_item_type", DbType.Int32, (int?)lootItemType),
                new Tuple<string, DbType, object>("loot_quantity", DbType.Int32, lootQuantity));
        }

        /// <summary>
        /// Creates a <see cref="Sprites.Screen"/> in the database.
        /// </summary>
        /// <remarks><see cref="Sprites.Screen._neighboringScreens"/> must be set with <see cref="SetNeighboringScreens(int, int, int, int, int)"/>.</remarks>
        /// <param name="renderType"><see cref="RenderType"/></param>
        /// <param name="renderValues">Values required to instanciate a <see cref="Sprites.Sprite.Render"/> (filename, color, and so forth).</param>
        /// <param name="darknessOpacity"><see cref="Sprites.Screen.DarknessOpacity"/></param>
        /// <param name="floorType"><see cref="Sprites.Floor.FloorType"/></param>
        /// <returns><see cref="Sprites.Sprite.Id"/></returns>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidRenderExceptionMessage"/></exception>
        /// <exception cref="NotImplementedException"><see cref="Messages.NotImplementedRenderExceptionMessage"/></exception>
        public int CreateScreen(RenderType renderType, object[] renderValues, FloorType floorType, double darknessOpacity)
        {
            string[] renderValuesString = CheckRenderValues(renderType, renderValues);

            darknessOpacity = darknessOpacity.Lower(0) ? 0 : (darknessOpacity.Greater(1) ? 1 : darknessOpacity);

            int id = GetNextId("screen");

            List<string> columnsList = ToColumnsArray(true, "floor_type", "darkness_opacity", "neighboring_screen_top",
                "neighboring_screen_bottom", "neighboring_screen_right", "neighboring_screen_left").ToList();
            List<DbType> typesList = ToTypesArray(true, DbType.Int32, DbType.Double, DbType.Int32,
                DbType.Int32, DbType.Int32, DbType.Int32).ToList();
            List<Object> valuesList = ToValuesArray(id, 0, 0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT, renderType, renderValuesString,
                floorType, darknessOpacity, 0, 0, 0, 0).ToList();

            // Removes the "screenId" value in each list.
            columnsList.RemoveAt(1);
            typesList.RemoveAt(1);
            valuesList.RemoveAt(1);

            ExecutePreparedInsert("screen",
                columnsList.ToArray(),
                typesList.ToArray(),
                valuesList.ToArray());

            return id;
        }

        /// <summary>
        /// Sets <see cref="Sprites.Screen._neighboringScreens"/> for a specified screen.
        /// </summary>
        /// <param name="screenId"><see cref="Sprites.Sprite.Id"/></param>
        /// <param name="screenIdTop"><see cref="Sprites.Screen._neighboringScreens"/> top identifier.</param>
        /// <param name="screenIdLeft"><see cref="Sprites.Screen._neighboringScreens"/> left identifier.</param>
        /// <param name="screenIdRight"><see cref="Sprites.Screen._neighboringScreens"/> right identifier.</param>
        /// <param name="screenIdBottom"><see cref="Sprites.Screen._neighboringScreens"/> bottom identifier.</param>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidSpriteIdExceptionMessage"/></exception>
        public void SetNeighboringScreens(int screenId, int screenIdTop, int screenIdLeft, int screenIdRight, int screenIdBottom)
        {
            if (!ExistsId("screen", screenId))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(screenId));
            }
            if (!ExistsId("screen", screenIdTop))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(screenIdTop));
            }
            if (!ExistsId("screen", screenIdLeft))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(screenIdLeft));
            }
            if (!ExistsId("screen", screenIdRight))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(screenIdRight));
            }
            if (!ExistsId("screen", screenIdBottom))
            {
                throw new ArgumentException(Messages.InvalidSpriteIdExceptionMessage, nameof(screenIdBottom));
            }

            using (SQLiteConnection connection = new SQLiteConnection(CONN_STRING))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "update screen set neighboring_screen_top = @neighboring_screen_top, " +
                        "neighboring_screen_bottom = @neighboring_screen_bottom, " +
                        "neighboring_screen_right = @neighboring_screen_right, " +
                        "neighboring_screen_left = @neighboring_screen_left " +
                        "where id = @id";

                    cmd.Parameters.Add("@neighboring_screen_top", DbType.Int32);
                    cmd.Parameters.Add("@neighboring_screen_bottom", DbType.Int32);
                    cmd.Parameters.Add("@neighboring_screen_right", DbType.Int32);
                    cmd.Parameters.Add("@neighboring_screen_left", DbType.Int32);
                    cmd.Parameters.Add("@id", DbType.Int32);

                    cmd.Parameters["@neighboring_screen_top"].Value = screenIdTop;
                    cmd.Parameters["@neighboring_screen_bottom"].Value = screenIdBottom;
                    cmd.Parameters["@neighboring_screen_right"].Value =screenIdRight ;
                    cmd.Parameters["@neighboring_screen_left"].Value = screenIdLeft;
                    cmd.Parameters["@id"].Value = screenId;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
