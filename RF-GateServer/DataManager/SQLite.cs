using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using Common;

namespace RF_GateServer.DataManager
{
    class SQLite
    {
        private string dbName = "server.sdb";
        private string filePath = "";
        private string connectString = "";
        private static SQLite _current = new SQLite();

        private SQLite()
        {
        }

        public static SQLite Current
        {
            get
            {
                return _current;
            }
        }

        public void Init()
        {
            filePath = AppDomain.CurrentDomain.BaseDirectory + dbName;
            if (File.Exists(filePath) == false)
            {
                SQLiteConnection.CreateFile(filePath);
                CreateTable();
            }
            connectString = String.Format("Data Source={0};Pooling=true;FailIfMissing=true", filePath);
        }

        public bool TableIsExist(string tableName)
        {
            try
            {
                var sql = "SELECT count(*) from sqlite_master where type ='table' and name ='" + tableName + "' ";
                var strCount = this.ExecuteScalar(sql);
                return 1.Equals(strCount);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int ExecuteScalar(string sql, params SQLiteParameter[] parms)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(connectString))
            {
                cnn.Open();
                SQLiteCommand cmd = new SQLiteCommand(cnn);
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parms);
                object value = cmd.ExecuteScalar();
                return value.ToInt32();
            }
        }

        private void CreateTable()
        {
            if (!TableIsExist("ChannelStateHistory"))
            {
                string sql = "CREATE TABLE ChannelStateHistory(";
                sql += "UUID NVARCHAR(100) primary key,";
                sql += "Name NVARCHAR(100) not null,";
                sql += "Type nvarchar(100) not null,";
                sql += "IP NVARCHAR(100) not null,";
                sql += "DisconnectTime DateTime not null,";
                sql += "ConnectTime DateTime";
                sql += ")";
                this.ExecuteNonQuery(sql);
            }
        }

        private int IsDisconnect(string ip)
        {
            var sql = "Select Count(*) FROM ChannelStateHistory WHERE IP=@IP AND ConnectTime is null";
            SQLiteParameter[] parms = new SQLiteParameter[1]
            {
                new SQLiteParameter { ParameterName ="@IP",DbType = DbType.String, Value = ip},
            };
            var scalar = ExecuteScalar(sql, parms);
            return scalar;
        }

        public void ChannelDisconnect(string name, string ip, string type)
        {
            var scalar = IsDisconnect(ip);
            if (scalar == 0)
            {
                var sql = "Insert Into ChannelStateHistory(UUID,Name,IP,Type,DisconnectTime) Values(@UUID,@Name,@IP,@Type,@DisconnectTime)";
                SQLiteParameter[] parms = new SQLiteParameter[5]
                {
                new SQLiteParameter { ParameterName ="@UUID", DbType = DbType.String,Value = Utility.GUID()},
                new SQLiteParameter { ParameterName ="@Name", DbType = DbType.String,Value = name},
                new SQLiteParameter { ParameterName ="@Type", DbType = DbType.String,Value = type},
                new SQLiteParameter { ParameterName ="@IP", DbType = DbType.String, Value = ip},
                new SQLiteParameter { ParameterName ="@DisconnectTime", DbType = DbType.DateTime, Value = DateTime.Now}
                };
                ExecuteNonQuery(sql, parms);
            }
        }

        public void ChannelConnect(string ip)
        {
            var sql = "Update ChannelStateHistory Set ConnectTime=@ConnectTime WHERE IP=@IP AND ConnectTime is null";
            SQLiteParameter[] parms = new SQLiteParameter[2]
            {
                    new SQLiteParameter { ParameterName ="@ConnectTime",DbType = DbType.DateTime, Value = DateTime.Now},
                    new SQLiteParameter { ParameterName ="@IP",DbType = DbType.String, Value = ip}
            };
            ExecuteNonQuery(sql, parms);
        }

        public List<ChannelDisconnectModel> Query(string ip, int pageIndex, int pageSize, out int totalCount)
        {
            List<ChannelDisconnectModel> list = new List<ChannelDisconnectModel>();
            var sql = "SELECT * FROM ChannelStateHistory WHERE 1=1 ";
            var sqlCount = "SELECT Count(*) FROM ChannelStateHistory WHERE 1=1 ";
            if (!ip.IsEmpty())
            {
                sql += "AND IP='" + ip + "'";
                sqlCount += "AND IP='" + ip + "'";
            }

            totalCount = ExecuteScalar(sqlCount);

            var skip = (pageIndex - 1) * pageSize;
            var take = pageSize;

            sql += string.Format(" Limit {0},{1}", skip, take);
            using (var conn = new SQLiteConnection(connectString))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    ChannelDisconnectModel model = new ChannelDisconnectModel
                    {
                        Name = reader["Name"].ToString(),
                        Ip = reader["Ip"].ToString(),
                        ChannelType = reader["Type"].ToString(),
                        DisconnectTime = reader["DisconnectTime"].ToString().ToDateTime(),
                        ConnectTime = null
                    };
                    list.Add(model);
                }
            }
            return list;
        }

        public void Clear(DateTime start, DateTime end)
        {

        }

        private int ExecuteNonQuery(string sql, params SQLiteParameter[] parms)
        {
            using (var conn = new SQLiteConnection(connectString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parms);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
