using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Repositories
{
    public class NotifyRepository : IGenericRepository<Notify>
    {
        private readonly ApplicationDbContext _context;

        public NotifyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notify>> GetAllAsync()
        => await _context.Notifications.Where(n => !n.IsDeleted).ToListAsync();

        public async Task<List<Notify>> GetAllByUserIdAsync(string userId)
            => await _context.Notifications.Where(n => n.UserId == userId && !n.IsDeleted).ToListAsync();

        public async Task<Notify> GetByIdAsync(int id)
            => await _context.Notifications.FindAsync(id) ?? throw new NullReferenceException("Notify not found");

        public async Task AddAsync(Notify notify)
        {
            await _context.AddAsync(notify);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Notify notify)
        {
            var trackedNotiy = _context.ChangeTracker.Entries<Notify>()
                                                 .FirstOrDefault(e => e.Entity.Id == notify.Id);

            //detached same id obj
            if (trackedNotiy != null)
            {
                _context.Entry(trackedNotiy.Entity).State = EntityState.Detached;
            }

            _context.Entry(notify).State = EntityState.Modified;
            notify.IsDeleted = true;
            await _context.SaveChangesAsync();

            //detached tracking obj after modified
            _context.Entry(notify).State = EntityState.Detached;
        }


        public async Task UpdateAsync(Notify notify)
        {
            var trackedNotiy = _context.ChangeTracker.Entries<Notify>()
                                     .FirstOrDefault(e => e.Entity.Id == notify.Id);

            //detached same id obj
            if (trackedNotiy != null)
            {
                _context.Entry(trackedNotiy.Entity).State = EntityState.Detached;
            }

            _context.Entry(notify).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //detached tracking obj after modified
            _context.Entry(notify).State = EntityState.Detached;
        }
    }
}
