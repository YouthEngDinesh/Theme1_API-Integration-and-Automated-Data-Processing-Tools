using ServerApp.Models;
using ServerApp.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Services
{
    class DeviceLogService
    {
        //private readonly DeviceLogRepository _repo;

        //public DeviceLogService()
        //{
        //    _repo = new DeviceLogRepository();
        //}

        //public List<DeviceLog> GetErrorRecords()
        //{
        //    return _repo.GetAll()
        //                .Where(x =>
        //                   !string.IsNullOrWhiteSpace(
        //                        x.ErrorCode))
        //                .ToList();
        //}

        //public List<DeviceLog> GetAmountDesc()
        //{
        //    return _repo.GetAll()
        //                .OrderByDescending(
        //                    x => x.Amount)
        //                .ToList();
        //}

        //public decimal GetAverage()
        //{
        //    return _repo.GetAll()
        //                .Where(x =>
        //                    string.IsNullOrEmpty(
        //                        x.ErrorCode))
        //                .Average(x =>
        //                    x.Amount);
        //}

    }
}
