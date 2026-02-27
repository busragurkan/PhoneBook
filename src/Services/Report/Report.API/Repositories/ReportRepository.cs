using Microsoft.EntityFrameworkCore;
using Report.API.Data;

namespace Report.API.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ReportDbContext _context;

    public ReportRepository(ReportDbContext context)
    {
        _context = context;
    }

    public async Task<List<Entities.Report>> GetAllAsync()
    {
        return await _context.Reports
            .AsNoTracking()
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task<Entities.Report?> GetByIdAsync(Guid id)
    {
        return await _context.Reports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Entities.Report> CreateAsync(Entities.Report report)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task UpdateAsync(Entities.Report report)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync();
    }
}
