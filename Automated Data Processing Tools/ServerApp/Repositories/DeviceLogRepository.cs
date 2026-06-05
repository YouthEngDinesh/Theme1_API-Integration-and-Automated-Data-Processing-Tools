using Dapper;
using Microsoft.Data.SqlClient;
using Serilog;
using ServerApp.Models;
using ServerApp.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ServerApp.Repositories
{
    class DeviceLogRepository
    {
        //DB接続
        //SQL　一覧表示　追加　更新　削除

        private readonly string _connectionString;

        public DeviceLogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }





        public async Task<IEnumerable<DeviceLog>> GetAll()
        {
            try
            {
                Log.Information("GetAll start");
                using var con = new SqlConnection(_connectionString);

                return await con.QueryAsync<DeviceLog>("SELECT * FROM DeviceLogs");
            }
            catch (Exception ex)
            {
                Log.Error(ex,"GetAll failed");
                return Enumerable.Empty<DeviceLog>();
            }
        }



        public async Task<int> Add(DeviceLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            Log.Information("Add start: {@Log}", log);


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

            return await con.ExecuteAsync(sql, log);
        }




        public  async Task<int> Update(DeviceLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            Log.Information("Update start: {@Log}", log);

            using var con = new SqlConnection(_connectionString);

            string sql = @"
            UPDATE DeviceLogs
            SET
                DeviceName=@DeviceName,
                Amount=@Amount,
                ErrorCode=@ErrorCode
            WHERE Id=@Id";

            return await con.ExecuteAsync(sql, log);
        }





        public async Task<int> Delete(int id)
        {
            Log.Information("Delete id: {Id}", id);

            using var con = new SqlConnection(_connectionString);

            return await con.ExecuteAsync(
                "DELETE FROM DeviceLogs WHERE Id=@Id",
                new { Id = id });
        }
    }


}
