using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Models;
using Server.Hubs;
using Shared;
using System;

namespace Server.Services
{
    public class DeviceDataService
    {
        private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;
        private readonly IHubContext<DataUpdateHub> dataUpdateHubContext;

        public DeviceDataService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IHubContext<DataUpdateHub> dataUpdateHubContext)
        {
            this.dbContextFactory = dbContextFactory;
            this.dataUpdateHubContext = dataUpdateHubContext;
        }

        private async Task SendDataUpdateMessage()
        {
            await dataUpdateHubContext.Clients.All.SendAsync("UpdateData");
        }

        /// <summary>
        /// Returns all saved data from device specified by deviceId
        /// </summary>
        /// <param name="deviceId">Unique device id</param>
        /// <returns></returns>
        public async Task<List<DeviceData>> GetAllDeviceDataAsync(string deviceId)
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            return await appDbContext.DeviceData.Where(dd => dd.DeviceId == deviceId).Include(dd => dd.Data).ToListAsync();
        }

        /// <summary>
        /// Returns latest x entries (specified by amount) from device specified by deviceId
        /// </summary>
        /// <param name="deviceId">Unique device id</param>
        /// <param name="amount">Number of latest entries to return</param>
        /// <returns></returns>
        public async Task<List<DeviceData>> GetLatestXDeviceDataAsync(string deviceId, int amount)
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            var a = appDbContext.DeviceData.Where(dd => dd.DeviceId == deviceId);
            var b = a.OrderByDescending(dd => dd.Timestamp);
            var c = b.Take(amount).Include(dd => dd.Data);
            return await c.ToListAsync();
        }

        /// <summary>
        /// Saves data from device into the database
        /// </summary>
        /// <param name="senderId">Unique device id</param>
        /// <param name="timestamp">Data timestamp</param>
        /// <param name="data">Actual data</param>
        /// <returns></returns>
        public async Task SaveDeviceDataAsync(string senderId, DateTime timestamp, DeviceDataBase data)
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            var deviceData = new DeviceData() { DeviceId = senderId, Timestamp = timestamp.ToUniversalTime(), Data = data };
            await appDbContext.AddAsync(deviceData);
            await appDbContext.SaveChangesAsync();
            Thread.Sleep(500);
            await SendDataUpdateMessage();
        }
    }
}
