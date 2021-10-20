using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

using TShockAPI;
using TShockAPI.DB;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;

namespace VoucherSystem
{
    class DBManager
    {
        private IDbConnection db;
        public DBManager()
        {
            switch (TShock.Config.Settings.StorageType.ToLower())
            {
                case "mysql":
                    string[] host = TShock.Config.Settings.MySqlHost.Split(':');
                    db = new MySqlConnection()
                    {
                        ConnectionString = String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                        host[0],
                        host.Length == 1 ? "3306" : host[1],
                        TShock.Config.Settings.MySqlDbName,
                        TShock.Config.Settings.MySqlUsername,
                        TShock.Config.Settings.MySqlPassword)
                    };
                    break;
                case "sqlite":
                    string dbPath = Path.Combine(TShock.SavePath, "VoucherSystem.sqlite");
                    db = new SqliteConnection(String.Format("uri=file://{0},Version=3", dbPath));
                    break;
            }
            SqlTableCreator creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ?
                (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            creator.EnsureTableStructure(new SqlTable("VoucherSystem",
                new SqlColumn("Id", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
                new SqlColumn("SerialNumber", MySqlDbType.Text),
                new SqlColumn("Reward", MySqlDbType.Text),
                new SqlColumn("CreatedBy", MySqlDbType.Text),
                new SqlColumn("ClaimedBy", MySqlDbType.Text),
                new SqlColumn("Expiration", MySqlDbType.Text)));
        }

        public bool AddVoucher(string serialnum, string reward, string author, string claimer, string expire)
        {
            try
            {
                return db.Query("INSERT INTO VoucherSystem (SerialNumber, Reward, CreatedBy, ClaimedBy, Expiration) " +
                    "VALUES (@0, @1, @2, @3, @4, @5)", serialnum, reward, author, claimer, expire) != 0;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.ToString());
            }
            return false;
        }

        public bool DeleteVoucher(string id)
        {
            try
            {
                return db.Query("DELETE FROM VoucherSystem WHERE Id = @0", id) != 0;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.ToString());
            }
            return false;
        }

    }
}
