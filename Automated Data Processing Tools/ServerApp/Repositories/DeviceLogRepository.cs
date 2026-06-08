using Dapper;
using Microsoft.Data.SqlClient;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace ServerApp.Repositories
{
    // DeviceLogテーブルへのCRUD処理を担当
    class DeviceLogRepository
    {
        private readonly string _connectionString = @"Server=(localdb)\MSSQLLocalDB;
                                                         Database=TeamTask1_test;
                                                         Trusted_Connection=True;
                                                         TrustServerCertificate=True;";

        // 全件データを取得
        public List<DeviceLog> GetAll()

        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<DeviceLog>("SELECT * FROM DeviceLogs").ToList();
        }

        // データを新規登録
        public void Add(DeviceLog log)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = """
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
       　　　　　　　　　　     GETDATE()
       　　　　　　　　　　 )
       　　　　　　　　　　 """;

            connection.Execute(sql, log);
        }

        // 指定データを更新
        public void Update(DeviceLog log)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = """
       　　　　　　　　　　 UPDATE DeviceLogs
       　　　　　　　　　　 SET
       　　　　　　　　　　     DeviceName=@DeviceName,
       　　　　　　　　　　     Amount=@Amount,
       　　　　　　　　　　     ErrorCode=@ErrorCode
       　　　　　　　　　　 WHERE
       　　　　　　　　　　     Id=@Id
       　　　　　　　　　　 """;

            connection.Execute(sql, log);
        }

        // 指定IDのデータを削除
        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Execute("DELETE FROM DeviceLogs WHERE Id=@Id",
                               new { Id = id });

        }
    }
}
