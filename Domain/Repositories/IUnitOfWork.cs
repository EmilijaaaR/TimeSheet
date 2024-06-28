namespace Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IClientRepository ClientRepository { get; }
        ICountryRepository CountryRepository { get; }
        IProjectRepository ProjectRepository { get; }
        ITimesheetRepository TimesheetRepository { get; }
        IUserRepository UserRepository { get; }
        IReportRepository ReportRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
