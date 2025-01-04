using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Repositories
{
    public class ReportRepository : IGenericRepository<Report>
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Report report)
        {
            _context.Reports.Add(report);
            _context.SaveChanges();
        }

        public void Update(Report report)
        {
            // Gán lại trạng thái cho đối tượng là modified và lưu các thay đổi
            _context.Entry(report).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Report report)
        {
            _context.Entry(report).State = EntityState.Modified;
            report.IsDeleted = true;
            _context.SaveChanges();
        }

        public List<Report> GetAll()
            => _context.Reports.AsNoTracking()
                                        .Where(r => !r.IsDeleted)
                                        .ToList();

        public Report GetById(int id)
            => _context.Reports.AsNoTracking()
                                        .FirstOrDefault(r => !r.IsDeleted && r.Id == id)
                                        ?? throw new NullReferenceException("Report not found");


    }
}
