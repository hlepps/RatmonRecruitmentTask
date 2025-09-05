using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.Data.Models;
using Shared;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Device> RegisteredDevices { get; set; }
        public DbSet<DeviceData> DeviceData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeviceDataBase>()
                .HasDiscriminator<string>("DeviceType")
                .HasValue<DeviceData_MOUSE2>("MOUSE2")
                .HasValue<DeviceData_MOUSE2B>("MOUSE2B")
                .HasValue<DeviceData_MOUSECOMBO>("MOUSECOMBO")
                .HasValue<DeviceData_MAS2>("MAS2");

            var reflectogramArrayValueConverter = new ValueConverter<List<Reflectogram>, string>(
            r => JsonSerializer.Serialize(r, JsonSerializerOptions.Default),
            s => JsonSerializer.Deserialize<List<Reflectogram>>(s, JsonSerializerOptions.Default));

            modelBuilder.Entity<DeviceData_MOUSECOMBO>().Property(x => x.Reflectograms).HasConversion(reflectogramArrayValueConverter);
        }
    }
}
