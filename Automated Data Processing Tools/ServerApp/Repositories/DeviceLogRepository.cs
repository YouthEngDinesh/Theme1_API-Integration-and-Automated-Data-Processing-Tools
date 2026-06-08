using Dapper;
using Microsoft.Data.SqlClient;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace ServerApp.Repositories
{
    class DeviceLogRepository
    {
        private readonly string _connectionString = @"Server=(localdb)\MSSQLLocalDB;
                                                         Database=TeamTask1_test;
                                                         Trusted_Connection=True;
                                                         TrustServerCertificate=True;";
        public List<DeviceLog> GetAll()

        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<DeviceLog>("SELECT * FROM DeviceLogs").ToList();
        }

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

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Execute("DELETE FROM DeviceLogs WHERE Id=@Id",
                               new { Id = id });

        }
    }
}
