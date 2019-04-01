using Boysenberry.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;

namespace Boysenberry.Repos
{
    class DataAccess
    {
        private static string DB;
        public DataAccess(string db)
        {
            DB = db;
        }
        public void CreateTable()
        {
            using (var db = new SQLiteConnection(DB))
            {
                db.Open();
                SQLiteCommand sqliteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText =
                    @"CREATE TABLE IF NOT EXISTS Record
                    (
                        UserId NVARCHAR(100) PRIMARY KEY,
                        Nickname NVARCHAR(2048) NULL,
                        Count INTEGER NULL,
                        UpdateTime INTEGER NULL
                    )"
                };
                sqliteCommand.ExecuteReader();
                //db.Close();
            }
        }
        public ObservableCollection<Record> QueryAll()
        {
            using (var db = new SQLiteConnection(DB))
            {
                db.Open();
                ObservableCollection<Record> list = new ObservableCollection<Record>();
                SQLiteCommand sqliteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = @"SELECT UserId,Nickname,Count,UpdateTime from Record ORDER BY UserId Asc"
                };
                SQLiteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    Record record = new Record
                    {
                        UserId = query.GetString(0),
                        Nickname = query.GetString(1),
                        Count = query.GetInt32(2),
                        UpdateTime = query.GetDateTime(3)
                    };
                    list.Add(record);
                }
                //db.Close();
                return list;
            }
        }
        public int Insert(Record record)
        {
            using (var db = new SQLiteConnection(DB))
            {
                db.Open();
                SQLiteCommand sqliteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = "INSERT INTO Record(UserId,Nickname,Count,UpdateTime) " +
                    "SELECT @UserId, @Nickname, @Count, @UpdateTime WHERE NOT EXISTS (SELECT 1 FROM Record WHERE UserId = @UserId);"
                };
                sqliteCommand.Parameters.AddWithValue("@UserId", record.UserId);
                sqliteCommand.Parameters.AddWithValue("@Nickname", record.Nickname);
                sqliteCommand.Parameters.AddWithValue("@Count", record.Count);
                sqliteCommand.Parameters.AddWithValue("@UpdateTime", record.UpdateTime);
                SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
                //db.Close();
                return sqliteDataReader.RecordsAffected;
            }
        }
        public void Update(Record record)
        {
            using (var db = new SQLiteConnection(DB))
            {
                db.Open();
                SQLiteCommand sqliteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = @"UPDATE Record SET
                                        Nickname = @Nickname,
                                        Count = @Count,
                                        UpdateTime = @UpdateTime
                                        WHERE UserId = @UserId
                                        ;"
                };
                sqliteCommand.Parameters.AddWithValue("@Nickname", record.Nickname);
                sqliteCommand.Parameters.AddWithValue("@Count", record.Count);
                sqliteCommand.Parameters.AddWithValue("@UpdateTime", record.UpdateTime);
                sqliteCommand.Parameters.AddWithValue("@UserId", record.UserId);
                sqliteCommand.ExecuteReader();
                //db.Close();
            }
        }
        public void Delete(Record record)
        {
            using (var db = new SQLiteConnection(DB))
            {
                db.Open();
                SQLiteCommand sqliteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = @"DELETE FROM Record WHERE UserId = @UserId;"
                };
                sqliteCommand.Parameters.AddWithValue("@UserId", record.UserId);
                sqliteCommand.ExecuteReader();
                //db.Close();
            }
        }
    }
}
