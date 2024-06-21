using DYUtil;
using GameLogicServer.Datas.Database;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace GameLogicServer
{
    public static class DatabaseConnector
    {
        static readonly string USER_ID = "root";
        static readonly string MYSQL_SERVER_DNS = "ec2-43-203-40-214.ap-northeast-2.compute.amazonaws.com";
        static readonly string DB_NAME = "network_final_multi";
        static readonly string PASSWORD = "0426";
        static MySqlConnection? connection = null;

        /* Database Tables */
        static readonly string USER_LOGIN_INFO_TABLE = "UserLoginInfo";

        public static void SetMySqlConnection()
        {
            try
            {
                string strConn = $"host={MYSQL_SERVER_DNS};Port=3306;Database={DB_NAME};UserName={USER_ID};Pwd={PASSWORD}";
                connection = new MySqlConnection(strConn);
                connection?.Open();
            }
            catch (Exception ex)
            {
                Logger.LogError("ERROR", ex.Message, true);
            }
            finally
            {
                connection?.Close();
                Logger.Log("SetMySQL", "Set SQL Connection Success", ConsoleColor.Green);
            }
        }
        public static bool TryCheckAccountExist(string id, string pw, out string nickName)
        {
            Debug.Assert(!string.IsNullOrEmpty(id));
            Debug.Assert(!string.IsNullOrEmpty(pw));
            nickName = null;

            try
            {
                connection?.Open();
                /* Ex: SELECT nickName FROM UserLoginInfo WHERE id = 'kdy0426' && password = '0426' */
                string query = $"SELECT nickName FROM {USER_LOGIN_INFO_TABLE} WHERE id = @id && password = @pw";
                Logger.Log("MySQL", query);
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pw", pw);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nickName = reader.GetString(0);
                    }
                    else
                    {
                        connection?.Close();
                        return false;
                    }
                }
                connection?.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("SQL ERROR", ex.Message);
            }
            finally
            {
                connection?.Close();
            }
            
            return false;
        }
        public static bool HasUserId(string id)
        {
            return HasData<DB_UserGameData>($"Id = \'{id}\'");
        }
        public static bool TryCreateAccount(DB_UserLoginInfo info)
        {
            try
            {
                InsertData<DB_UserLoginInfo>(info);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateAccount", ex.Message);
                return false;
            }
        }
        public static DB_UserGameData GetUserGameData(string id)
        {
            Debug.Assert(!string.IsNullOrEmpty(id));
            if (HasData<DB_UserGameData>($"Id = \'{id}\'"))
            {
                Debug.Assert(connection != null);
                try
                {
                    connection.Open();
                    string sql = $"SELECT * FROM {GetTableName<DB_UserGameData>()} WHERE Id = @id";
                    Logger.Log("MySQL", sql);
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userId = reader.GetString("Id");
                            long sumPoint = reader.GetInt64("SumPoint");
                            int maxPoint = reader.GetInt32("MaxPoint");
                            return new DB_UserGameData(userId, sumPoint, maxPoint);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                DB_UserGameData data = new DB_UserGameData(id, 0, 0);
                InsertData<DB_UserGameData>(data);
                return data;
            }
            return null;
        }
        public static bool TryCraeteRoom(DB_GameRoom room)
        {
            try
            {
                if (!InsertData<DB_GameRoom>(room))
                {
                    throw new Exception("SQL Query 문제!!!");
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Create Room", ex.Message);
                return false;
            }
        }
        public static bool HasRoomName(string roomName)
        {
            return HasData<DB_GameRoom>($"RoomName = \'{roomName}\'");
        }
        public static List<DB_GameRoom> GetAllGameRoom()
        {
            return GetData<DB_GameRoom>("1");
        }
        public static DB_GameRoom GetGameRoom(string roomName)
        {
            if (!HasRoomName(roomName))
            {
                return null;
            }
            List<DB_GameRoom> roomList = GetData<DB_GameRoom>($"RoomName = \'{roomName}\'");
            if (roomList.Count > 0)
            {
                return roomList[0];
            }
            return null;
        }
        public static bool TryJoinRoom(DB_RoomUserInfo roomUserInfo)
        {
            InsertData<DB_RoomUserInfo>(roomUserInfo);
            return false;
        }
        private static bool HasData<T>(string condition)
        {
            Debug.Assert(!string.IsNullOrEmpty(condition));
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM {GetTableName<T>()} WHERE {condition}";
                Logger.Log("MySQL", sql);
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
        private static List<T> GetData<T>(string condition) where T : new()
        {
            Debug.Assert(!string.IsNullOrEmpty(condition));
            Debug.Assert(connection != null);

            List<T> results = new List<T>();

            try
            {
                connection.Open();
                string tableName = GetTableName<T>();
                string sql = $"SELECT * FROM {tableName} WHERE {condition}";
                Logger.Log("MySQL", sql);
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T instance = new T();
                        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                        foreach (var property in typeof(T).GetProperties(bindingFlags))
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                            {
                                var value = reader.GetValue(reader.GetOrdinal(property.Name));
                                property.SetValue(instance, value);
                            }
                        }
                        results.Add(instance);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }

            return results;
        }
        private static bool InsertData<T>(T table)
        {
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string tableName = GetTableName<T>();
                StringBuilder columnsBuilder = new StringBuilder();
                StringBuilder valuesBuilder = new StringBuilder();
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                foreach (var property in table.GetType().GetProperties(bindingFlags))
                {
                    columnsBuilder.Append(property.Name);
                    columnsBuilder.Append(", ");

                    var value = property.GetValue(table);
                    if (value != null)
                    {
                        switch (property.PropertyType.ToString())
                        {
                            case "System.DateTime":
                                valuesBuilder.Append(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
                                break;
                            case "System.String":
                                valuesBuilder.Append("\'");
                                valuesBuilder.Append(value.ToString());
                                valuesBuilder.Append("\'");
                                break;
                            default:
                                valuesBuilder.Append(value.ToString());
                                break;
                        }
                    }
                    else
                    {
                        valuesBuilder.Append("NULL");
                    }
                    valuesBuilder.Append(", ");
                }
                // ", " 들 제거
                columnsBuilder.Length -= 2;
                valuesBuilder.Length -= 2;
                string sql = $"INSERT INTO {tableName}({columnsBuilder.ToString()}) VALUES({valuesBuilder.ToString()})";
                Logger.Log("MySQL", sql);
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            finally
            {
                connection?.Close();
            }
            return true;
        }
        private static void SelectData<T>(string condition)
        {
            Debug.Assert(connection != null);
            try
            {
                connection?.Open();
                string tableName = GetTableName<T>();
                string sql = $"SELECT * FROM {tableName} WHERE {condition}";
                Logger.Log("MySQL", sql);
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("==========");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"{reader.GetName(i)}: {reader.GetValue(i)}");
                        }
                        Console.WriteLine("==========");
                    }
                }
                connection?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
        }
        private static string GetTableName<T>()
        {
            string className = typeof(T).Name;
            int underscoreIndex = className.IndexOf('_');
            Debug.Assert(underscoreIndex != -1, "[ERROR] Class 이름에 _가 존재하지 않습니다.\nmySQL 클래스명 규칙: [DB명]_[Table명]");

            return className.Substring(underscoreIndex + 1);
        }
    }
}
