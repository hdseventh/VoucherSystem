using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TShockAPI;
using TShockAPI.DB;

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
                new SqlColumn("Expiration", MySqlDbType.DateTime)));
        }

        public bool AddVoucher(string serialnum, string reward, string author, string claimer, DateTime expire)
        {
            try
            {
                return db.Query("INSERT INTO VoucherSystem (SerialNumber, Reward, CreatedBy, ClaimedBy, Expiration) " +
                    "VALUES (@0, @1, @2, @3, @4)", serialnum, reward, author, claimer, expire) != 0;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.ToString());
            }
            return false;
        }

        public bool UpdateVoucher(int Id, string claimer)
        {
            try
            {
                return db.Query("UPDATE VoucherSystem SET ClaimedBy=@1 WHERE Id=@0", Id, claimer) != 0;
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

        public List<VoucherSystems> GetVoucherList()
        {
            try
            {
                // list all rows in database
                List<VoucherSystems> vlist = new List<VoucherSystems>();
                using (var reader = db.QueryReader("SELECT * FROM VoucherSystem WHERE ClaimedBy=\"\""))
                {
                    while (reader.Read())
                    {
                        vlist.Add(new VoucherSystems(reader.Get<int>("Id"), reader.Get<string>("SerialNumber"),
                            reader.Get<string>("Reward"), reader.Get<string>("CreatedBy"), reader.Get<string>("ClaimedBy"), reader.Get<string>("Expiration")));
                    }
                }
                return vlist;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.ToString());
            }

            return new List<VoucherSystems>();
        }

        public VoucherSystems ClaimVoucher(string check)
        {
            try
            {
                // search for matching range in database
                List<VoucherSystems> vlist = GetVoucherList();
                foreach (VoucherSystems voucher in vlist)
                {
                    if (check == voucher.SerialNumber)
                        return voucher;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.ToString());
            }

            return null;
        }

    }

    public class VoucherSystems
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Reward { get; set; }
        public string CreatedBy { get; set; }
        public string ClaimedBy { get; set; }
        public string Expiration { get; set; }

        // class constructors
        public VoucherSystems(int Id, string serial, string reward, string author, string claimer, string expire)
        {
            this.Id = Id;
            this.SerialNumber = serial;
            this.Reward = reward;
            this.CreatedBy = author;
            this.ClaimedBy = claimer;
            this.Expiration = expire;
        }

        public VoucherSystems()
        {
            this.Id = 0;
            this.SerialNumber = "";
            this.Reward = "";
            this.CreatedBy = "";
            this.ClaimedBy = "";
            this.Expiration = "";
        }
    }
}
