namespace Report.API.Repositories;

public interface IReportRepository
{
    Task<List<Entities.Report>> GetAllAsync();
    Task<Entities.Report?> GetByIdAsync(Guid id);
    Task<Entities.Report> CreateAsync(Entities.Report report);
    Task UpdateAsync(Entities.Report report);
}
