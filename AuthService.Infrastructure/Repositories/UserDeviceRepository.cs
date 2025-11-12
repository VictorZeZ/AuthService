using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class UserDeviceRepository : IUserDeviceRepository
    {
        private readonly AppDbContext _context;

        public UserDeviceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserDevice?> GetByIdAsync(Guid id)
        {
            return await _context.UserDevices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<UserDevice?> GetByDeviceIdAsync(string deviceId)
        {
            return await _context.UserDevices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        public async Task<IEnumerable<UserDevice>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserDevices
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserDevice?> GetByUserIdAndDeviceInfoAsync(Guid userId, string deviceInfo)
        {
            return await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceInfo == deviceInfo);
        }

        public async Task<UserDevice?> GetValidDeviceAsync(Guid userId, string deviceId)
        {
            return await _context.UserDevices
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.UserId == userId &&
                    d.DeviceId == deviceId &&
                    d.ExpiresAt > DateTime.UtcNow);
        }


        public async Task AddAsync(UserDevice device)
        {
            await _context.UserDevices.AddAsync(device);
        }

        public async Task UpdateAsync(UserDevice device)
        {
            _context.UserDevices.Update(device);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(UserDevice device)
        {
            _context.UserDevices.Remove(device);
            await Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
