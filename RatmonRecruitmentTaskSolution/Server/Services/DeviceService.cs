using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Models;
using Server.Hubs;
using Shared;

namespace Server.Services
{
    public class DeviceService
    {
        private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;
        private readonly IHubContext<DataUpdateHub> dataUpdateHubContext;

        public DeviceService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IHubContext<DataUpdateHub> dataUpdateHubContext)
        {
            this.dbContextFactory = dbContextFactory;
            this.dataUpdateHubContext = dataUpdateHubContext;
        }
        private async Task SendDataUpdateMessageAsync()
        {
            await dataUpdateHubContext.Clients.All.SendAsync("UpdateData");
        }

        /// <summary>
        /// Returns list of all registered devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<Device>> GetAllRegisteredDevicesAsync()
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            return await appDbContext.RegisteredDevices.ToListAsync();
        }

        /// <summary>
        /// Returns registered Device object specified by deviceId
        /// </summary>
        /// <param name="deviceId">Unique device id</param>
        /// <returns></returns>
        public async Task<Device> GetDeviceByIdAsync(string deviceId)
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            return await appDbContext.RegisteredDevices.SingleAsync(d => d.Id == deviceId);
        }

        /// <summary>
        /// Returns true if device specified by deviceId is registered in database
        /// </summary>
        /// <param name="deviceId">Unique device id</param>
        /// <returns></returns>
        public async Task<bool> CheckIfDeviceIsRegisteredAsync(string deviceId)
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            var amount = await appDbContext.RegisteredDevices.CountAsync(d => d.Id == deviceId);
            return amount != 0;
        }

        /// <summary>
        /// Registers new device in the database
        /// </summary>
        /// <param name="Id">Unique device id</param>
        /// <param name="Name">Device name</param>
        /// <param name="type">Device type</param>
        /// <returns></returns>
        public async Task RegisterNewDeviceAsync(string deviceId, string Name, DeviceType type)
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            await appDbContext.RegisteredDevices.AddAsync(new Device { Id = deviceId, Name = Name, Type = type });
            await appDbContext.SaveChangesAsync();
            await SendDataUpdateMessageAsync();
        }

        /// <summary>
        /// Updates name of device specified by deviceId
        /// </summary>
        /// <param name="deviceId">Unique device id</param>
        /// <param name="Name">Device name</param>
        /// <returns></returns>
        public async Task UpdateDeviceNameAsync(string deviceId, string Name)
        {
            using var appDbContext = dbContextFactory.CreateDbContext();
            appDbContext.RegisteredDevices.Single(d => d.Id == deviceId).Name = Name;
            await appDbContext.SaveChangesAsync();
            await SendDataUpdateMessageAsync();
        }
    }
}
