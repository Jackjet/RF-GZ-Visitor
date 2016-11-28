using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using Common;
using Common.Log;

namespace RF_GateServer.DataManager
{
    class SQLite
    {
        private string dbName = "server.sdb";
        private string filePath = "";
        private string connectString = "";
        private static SQLite _current = new SQLite();

        private const string disconnect_tableName = "ChannelState";
        private const string intout_tableName = "InOut";

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
            connectString = String.Format("Data Source={0};Pooling=true;FailIfMissing=true", filePath);
            if (File.Exists(filePath) == false)
            {
                SQLiteConnection.CreateFile(filePath);
                CreateTable();
            }
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
            if (!TableIsExist(disconnect_tableName))
            {
                string sql = "CREATE TABLE " + disconnect_tableName + "(";
                sql += "UUID NVARCHAR(100) primary key,";
                sql += "Name NVARCHAR(100) not null,";
                sql += "Type nvarchar(100) not null,";
                sql += "IP NVARCHAR(100) not null,";
                sql += "DisconnectTime DateTime not null,";
                sql += "ConnectTime DateTime";
                sql += ")";
                this.ExecuteNonQuery(sql);
            }

            if (!TableIsExist(intout_tableName))
            {
                string sql = "CREATE TABLE " + intout_tableName + "(";
                sql += "UUID NVARCHAR(100) primary key,";
                sql += "Name NVARCHAR(100) not null,";
                sql += "Type nvarchar(100) not null,";
                sql += "IP NVARCHAR(100) not null,";
                sql += "QRCode NVARCHAR(256) not null,";
                sql += "Status NVARCHAR(100) not null,";
                sql += "ElapseTime NVARCHAR(100) not null,";
                sql += "CheckTime DateTime not null";
                sql += ")";
                this.ExecuteNonQuery(sql);
            }
        }

        private int IsDisconnect(string ip)
        {
            var sql = "Select Count(*) FROM " + disconnect_tableName + " WHERE IP=@IP AND ConnectTime is null";
            SQLiteParameter[] parms = new SQLiteParameter[1]
            {
                new SQLiteParameter { ParameterName ="@IP",DbType = DbType.String, Value = ip},
            };
            var scalar = ExecuteScalar(sql, parms);
            return scalar;
        }

        public void Disconnect(string name, string ip, string type)
        {
            var scalar = IsDisconnect(ip);
            if (scalar == 0)
            {
                var sql = "Insert Into " + disconnect_tableName + "(UUID,Name,IP,Type,DisconnectTime) Values(@UUID,@Name,@IP,@Type,@DisconnectTime)";
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

        public void Connect(string ip)
        {
            var sql = "Update " + disconnect_tableName + " Set ConnectTime=@ConnectTime WHERE IP=@IP AND ConnectTime is null";
            SQLiteParameter[] parms = new SQLiteParameter[2]
            {
                    new SQLiteParameter { ParameterName ="@ConnectTime",DbType = DbType.DateTime, Value = DateTime.Now},
                    new SQLiteParameter { ParameterName ="@IP",DbType = DbType.String, Value = ip}
            };
            ExecuteNonQuery(sql, parms);
        }

        public List<DisconnectModel> QueryState(string ip, PageQuery page)
        {
            List<DisconnectModel> list = new List<DisconnectModel>();
            var sql = "SELECT * FROM " + disconnect_tableName + " WHERE 1=1 ";
            var sqlCount = "SELECT Count(*) FROM " + disconnect_tableName + " WHERE 1=1 ";
            var condition = "";
            if (!ip.IsEmpty())
            {
                condition += "AND IP='" + ip + "'";
            }
            sql += condition;
            sqlCount += condition;

            page.TotalCount = ExecuteScalar(sqlCount);

            var skip = (page.PageIndex - 1) * page.PageSize;
            var take = page.PageSize;

            sql += string.Format(" Limit {0},{1}", skip, take);
            using (var conn = new SQLiteConnection(connectString))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    DisconnectModel model = new DisconnectModel
                    {
                        Name = reader["Name"].ToString(),
                        Ip = reader["Ip"].ToString(),
                        ChannelType = reader["Type"].ToString(),
                        DisconnectTime = reader["DisconnectTime"].ToString().ToDateTime(),
                        ConnectTime = reader["ConnectTime"].ToString().ToDateTime()
                    };
                    list.Add(model);
                }
            }
            return list;
        }

        public void Clear(DateTime start, DateTime end)
        {

        }

        public void InOut(InOutModel entity)
        {
            var sql = "Insert Into " + intout_tableName + "(UUID,Name,IP,Type,QRCode,Status,ElapseTime,CheckTime) Values(@UUID,@Name,@IP,@Type,@QRCode,@Status,@ElapseTime,@CheckTime)";
            SQLiteParameter[] parms = new SQLiteParameter[8]
            {
                new SQLiteParameter { ParameterName ="@UUID", DbType = DbType.String,Value = Utility.GUID()},
                new SQLiteParameter { ParameterName ="@Name", DbType = DbType.String,Value = entity.Name},
                new SQLiteParameter { ParameterName ="@Type", DbType = DbType.String,Value = entity.ChannelType},
                new SQLiteParameter { ParameterName ="@IP", DbType = DbType.String, Value = entity.Ip},
                new SQLiteParameter { ParameterName ="@QRCode", DbType = DbType.String, Value = entity.QRCode},
                new SQLiteParameter { ParameterName ="@Status", DbType = DbType.String, Value = entity.Status},
                new SQLiteParameter { ParameterName ="@ElapseTime", DbType = DbType.String, Value = entity.ElapseTime},
                new SQLiteParameter { ParameterName ="@CheckTime", DbType = DbType.DateTime, Value = DateTime.Now}
            };
            ExecuteNonQuery(sql, parms);
        }

        public List<InOutModel> QueryInOut(string channelName, DateTime start, DateTime end, PageQuery page)
        {
            List<InOutModel> list = new List<InOutModel>();
            var sql = "SELECT * FROM " + intout_tableName + " WHERE 1=1 ";
            var sqlCount = "SELECT Count(*) FROM " + intout_tableName + " WHERE 1=1 ";
            var condition = "";
            if (!channelName.IsEmpty())
            {
                condition += "AND Name='" + channelName + "' ";
            }
            condition += string.Format(" AND CheckTime>='{0}' AND CheckTime <='{1}'", start.ToStandard(), end.ToStandard());

            sql += condition;
            sqlCount += condition;

            page.TotalCount = ExecuteScalar(sqlCount);

            var skip = (page.PageIndex - 1) * page.PageSize;
            var take = page.PageSize;

            sql += string.Format(" Limit {0},{1}", skip, take);
            using (var conn = new SQLiteConnection(connectString))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    InOutModel model = new InOutModel
                    {
                        Name = reader["Name"].ToString(),
                        Ip = reader["Ip"].ToString(),
                        ChannelType = reader["Type"].ToString(),
                        QRCode = reader["QRCode"].ToString(),
                        Status = reader["Status"].ToString(),
                        ElapseTime = reader["ElapseTime"].ToString(),
                        CheckTime = reader["CheckTime"].ToString().ToDateTime(),
                    };
                    list.Add(model);
                }
            }
            return list;
        }

        private int ExecuteNonQuery(string sql, params SQLiteParameter[] parms)
        {
            using (var conn = new SQLiteConnection(connectString))
            {
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn);
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parms);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogHelper.Info("执行sql失败->" + sql);
                    return -1;
                }
            }
        }
    }
}
