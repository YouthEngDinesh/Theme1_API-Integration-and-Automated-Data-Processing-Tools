using Dapper;
using Microsoft.Data.SqlClient;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace ServerApp.Repositories
{
    class DeviceLogRepository : DeviceLog
    {
        //DB接続
        //SQL　一覧表示　追加　更新　削除
        public string connectionString =
             "Server=(localdb)\\MSSQLLocalDB;Database=TestDB;Trusted_Connection=True;";

        private readonly string _connectionString;

        public DeviceLogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<DeviceLog> GetAll()
        {
            using var con = new SqlConnection(_connectionString);

            return con.Query<DeviceLog>(
                "SELECT * FROM DeviceLogs");
        }

        public int Add(DeviceLog log)
        {
            using var con = new SqlConnection(_connectionString);

            string sql = @"
        INSERT INTO DeviceLogs
        (
            DeviceName,
            Amount,
            ErrorCode,
            Timestamp
        )
        VALUES
        (
            @DeviceName,
            @Amount,
            @ErrorCode,
            @Timestamp
        )";

            return con.Execute(sql, log);
        }

        public int Update(DeviceLog log)
        {
            using var con = new SqlConnection(_connectionString);

            string sql = @"
            UPDATE DeviceLogs
            SET
                DeviceName=@DeviceName,
                Amount=@Amount,
                ErrorCode=@ErrorCode
            WHERE Id=@Id";

            return con.Execute(sql, log);
        }

        public int Delete(int id)
        {
            using var con = new SqlConnection(_connectionString);

            return con.Execute(
                "DELETE FROM DeviceLogs WHERE Id=@Id",
                new { Id = id });
        }
    }


}

