using DYUtil;
using MySql.Data.MySqlClient;
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

        private static void InsertData<T>(T table)
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
                        valuesBuilder.Append("\'");
                        switch (property.PropertyType.ToString())
                        {
                            case "System.DateTime":
                                valuesBuilder.Append(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
                                break;
                            default:
                                valuesBuilder.Append(value.ToString());
                                break;
                        }
                        valuesBuilder.Append("\'");
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
                Console.WriteLine($"[INPUT] {sql}");
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                connection?.Close();
            }
        }
        private static void SelectData<T>(string condition)
        {
            Debug.Assert(connection != null);
            try
            {
                connection?.Open();
                string tableName = GetTableName<T>();
                string sql = $"SELECT * FROM {tableName} WHERE {condition}";
                Console.WriteLine($"[INPUT] {sql}");
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
