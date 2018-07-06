using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class SqliteDB {

    private static string conString = string.Format("URI=file:{0}/Database/highscore.db", Application.dataPath);
    private static SqliteConnection cn = new SqliteConnection(conString);

    public static List<User> GetAll()
    {
        List<User> list = new List<User>();

        cn.Open();

        SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = "SELECT * FROM user ORDER BY score LIMIT 5";

        SqliteDataReader reader = cmd.ExecuteReader();

        while(reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            int score = reader.GetInt32(2);

            list.Add(new User(id, name, score));
        }

        reader.Close();
        cn.Close();

        return list;
    }

    public static bool Insert(User user)
    {
        cn.Open();

        SqliteCommand cmd = cn.CreateCommand();
        cmd.CommandText = "INSERT INTO user(name, score) VALUES(@name, @score)";

        cmd.Parameters.Add(new SqliteParameter {
            Value = user.name,
            ParameterName = "@name",
            DbType = DbType.String
        });

        cmd.Parameters.Add(new SqliteParameter
        {
            Value = user.score,
            ParameterName = "@score",
            DbType = DbType.Int32
        });

        int num = cmd.ExecuteNonQuery();

        cn.Close();

        return num > 0;
    }

    public static bool Delete(User user)
    {
        cn.Open();

        SqliteCommand cmd = cn.CreateCommand();
        cmd.CommandText = "DELETE FROM user WHERE id = @id";

        cmd.Parameters.Add(new SqliteParameter
        {
            Value = user.id,
            ParameterName = "@id",
            DbType = DbType.Int32
        });

        int num = cmd.ExecuteNonQuery();

        cn.Close();

        return num > 0;
    }
}
